using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Challenge.IncrementalLoading;
using Challenge.Model;
using Challenge.Rest;
using MvvmCross.Commands;
using MvvmCross.Plugin.WebBrowser;
using MvvmCross.ViewModels;
using Refit.Insane.PowerPack.Caching;
using Refit.Insane.PowerPack.Data;
using Refit.Insane.PowerPack.Services;

namespace Challenge.ViewModels
{
    public class PullRequestsViewModel : MvxViewModel<Repository>, ISupportIncrementalLoading
    {
        private readonly IRestService _restService;
        private readonly IMvxWebBrowserTask _webBrowserTask;

        private int _page = 1;

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private void ResetPageCounters()
        {
            HasMoreItems = false;
            LoadMoreTask = null;
            _page = 1;
        }

        public IMvxCommand RefreshCommand => new MvxAsyncCommand(
            async () =>
            {
                IsRefreshing = true;
                try
                {
                    await ClearRepositoriesCache();
                    await LoadPullRequests();
                }
                catch (Exception e)
                {
                    ResetPullRequests();
                    PullRequestsLoadTask = MvxNotifyTask.Create(Task.FromException(e));
                }
                finally
                {
                    IsRefreshing = false;
                }
            });

        private async Task ClearRepositoriesCache()
        {
            for (var i = 1; i <= 1000 / Constants.RefitPerPage; i++)
            {
                var page = i;
                var perPage = Constants.RefitPerPage;
                var user = Repository.Owner.Login;
                var repo = Repository.Name;
                var cacheKey = new PullRequestsCacheKey {Page = page, Repo = repo, User = user}.ToString();
                await RefitCacheService.Instance.ClearCache<IGitHubApi, IEnumerable<PullRequest>>(
                    api => api.GetPullRequests(
                        cacheKey,
                        page,
                        perPage,
                        user,
                        repo, default(CancellationToken)));
            }
        }

        private void ResetPullRequests()
        {
            PullRequests = new MvxObservableCollection<PullRequest>();
            ResetPageCounters();
        }

        private MvxObservableCollection<PullRequest> _pullRequests;

        public MvxObservableCollection<PullRequest> PullRequests
        {
            get => _pullRequests;
            set => SetProperty(ref _pullRequests, value);
        }

        public int PageSize { get; set; } = Constants.RefitPerPage;

        private MvxNotifyTask _loadMoreTask;

        public MvxNotifyTask LoadMoreTask
        {
            get => _loadMoreTask;
            set => SetProperty(ref _loadMoreTask, value);
        }

        private bool _hasMoreItems;

        public bool HasMoreItems
        {
            get => _hasMoreItems;
            set => SetProperty(ref _hasMoreItems, value);
        }

        private MvxNotifyTask _pullRequestsLoadTask;

        public MvxNotifyTask PullRequestsLoadTask
        {
            get => _pullRequestsLoadTask;
            set => SetProperty(ref _pullRequestsLoadTask, value);
        }

        public IMvxCommand OpenPullRequestsInBrowserCommand => new MvxCommand<PullRequest>(
            pullRequest =>
            {
                if (pullRequest?.HtmlUrl != null)
                {
                    _webBrowserTask.ShowWebPage(pullRequest.HtmlUrl);
                }
            });

        public IMvxCommand LoadMoreItemsCommand => new MvxCommand(
            () => { LoadMoreTask = MvxNotifyTask.Create(async () => await LoadMoreItems()); });

        private Repository _repository;

        public PullRequestsViewModel(IRestService restService, IMvxWebBrowserTask webBrowserTask)
        {
            _restService = restService;
            _webBrowserTask = webBrowserTask;
        }

        public Repository Repository
        {
            get => _repository;
            set => SetProperty(ref _repository, value);
        }

        public override void Prepare(Repository parameter)
        {
            Repository = parameter;
            PullRequestsLoadTask = MvxNotifyTask.Create(async () => await LoadPullRequests());
        }

        private async Task LoadPullRequests()
        {
            ResetPageCounters();
            var response = await GetPullRequests(Repository.Name, Repository.Owner.Login, _page);
            if (response.IsSuccess)
            {
                PullRequests = new MvxObservableCollection<PullRequest>(response.Results);
                HasMoreItems = true;
            }
            else
            {
                throw new Exception(response.FormattedErrorMessages);
            }
        }

        private async Task LoadMoreItems()
        {
            _page++;
            var response = await GetPullRequests(Repository.Name, Repository.Owner.Login, _page);
            if (response.IsSuccess)
            {
                if (response.Results.Any())
                {
                    PullRequests.AddRange(response.Results);
                }
                else
                {
                    HasMoreItems = false;
                }
            }
            else
            {
                throw new Exception(response.FormattedErrorMessages);
            }
        }

        private async Task<Response<IEnumerable<PullRequest>>> GetPullRequests(string repo, string user, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var perPage = Constants.RefitPerPage;
                var cacheKey = new PullRequestsCacheKey {Page = page, Repo = repo, User = user}.ToString();
                var response =
                    await _restService.Execute<IGitHubApi, IEnumerable<PullRequest>>(
                        api => api.GetPullRequests(
                            cacheKey,
                            page,
                            perPage,
                            user,
                            repo,
                            cancellationToken));
                if (response.IsSuccess && !response.Results.Any())
                {
                    throw new Exception(Resources.Texts.NoResultsFound);
                }

                return response;
            }
            catch (Exception e)
            {
                return new Response<IEnumerable<PullRequest>>().AddErrorMessage(e.Message).SetAsFailureResponse();
            }
        }

    }
}
