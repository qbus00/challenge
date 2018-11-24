using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Challenge.IncrementalLoading;
using Challenge.Model;
using Challenge.Rest;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Nito.AsyncEx;
using Refit.Insane.PowerPack.Data;
using Refit.Insane.PowerPack.Services;
using Xamarin.Forms;

namespace Challenge.ViewModels
{
    public class ReposViewModel : MvxViewModel, ISupportIncrementalLoading
    {
        private readonly IRestService _restService;
        private IDisposable _searchObservable;
        private readonly IMvxNavigationService _navigationService;

        private string _searchPhrase = string.Empty;
        public string SearchPhrase
        {
            get => _searchPhrase;
            set => SetProperty(ref _searchPhrase, value);
        }

        private MvxNotifyTask _repositoriesLoadTask;
        public MvxNotifyTask RepositoriesLoadTask
        {
            get => _repositoriesLoadTask;
            set => SetProperty(ref _repositoriesLoadTask, value);
        }

        private MvxNotifyTask _loadMoreTask;
        public MvxNotifyTask LoadMoreTask
        {
            get => _loadMoreTask;
            set => SetProperty(ref _loadMoreTask, value);
        }

        private int _page = 1;
        private int _totalCount;

        private MvxObservableCollection<Repository> _repositories;
        public MvxObservableCollection<Repository> Repositories
        {
            get => _repositories;
            set => SetProperty(ref _repositories, value);
        }

        public int PageSize { get; set; } = Constants.RefitPerPage;

        private bool _hasMoreItems;
        public bool HasMoreItems
        {
            get => _hasMoreItems;
            set => SetProperty(ref _hasMoreItems, value);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            SetupSearch();
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            _searchObservable?.Dispose();
        }

        private void SetupSearch()
        {
            _searchObservable = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler)
                .ObserveOn(SynchronizationContext.Current)
                .Where(pattern => pattern.EventArgs.PropertyName == nameof(SearchPhrase))
                .Select(pattern => SearchPhrase.Trim(' ', '\t'))
                .Throttle(TimeSpan.FromMilliseconds(Constants.SearchThrottlingInMiliseconds))
                .DistinctUntilChanged()
                .Select(searchPhrase => Observable.FromAsync(async token =>
                {
                    var searchDoneEvent = new AsyncAutoResetEvent();
                    var itemsChangedDoneEvent = new AsyncAutoResetEvent();
                    Response<ItemsCollection<Repository>> result = null;
                    RepositoriesLoadTask = MvxNotifyTask.Create(async () =>
                    {
                        result = await GetRepositories(searchPhrase, 1);
                        searchDoneEvent.Set();
                        await itemsChangedDoneEvent.WaitAsync();
                        await Task.Delay(50);
                        if (!result.IsSuccess)
                        {
                            throw new Exception(result.FormattedErrorMessages);
                        }
                    });
                    await searchDoneEvent.WaitAsync(token);
                    return (Result: result, ItemsChangedDoneEvent: itemsChangedDoneEvent);
                })).Switch().Subscribe(observer =>
                {
                    try
                    {
                        ResetPageCounters();
                        if (observer.Result.IsSuccess)
                        {
                            Repositories = new MvxObservableCollection<Repository>(observer.Result.Results.Items);
                            _totalCount = observer.Result.Results.TotalCount;
                            HasMoreItems = _totalCount > Constants.RefitPerPage;
                        }
                        else
                        {
                            Repositories = new MvxObservableCollection<Repository>();
                            _totalCount = 0;
                            HasMoreItems = false;
                        }
                    }
                    finally
                    {
                        observer.ItemsChangedDoneEvent.Set();
                    }
                });
        }

        public IMvxCommand RefreshCommand => new MvxCommand(
            () =>
            {
            });

        public IMvxCommand GotoPullRequestsCommand => new MvxCommand<Repository>(
            repository => { _navigationService.Navigate<PullRequestsViewModel, Repository>(repository); });

        public IMvxCommand LoadMoreItemsCommand => new MvxCommand(
            () => { LoadMoreTask = MvxNotifyTask.Create(async () => await LoadMoreItems()); });

        public string Title => Resources.Texts.MainPageTitle;

        public ReposViewModel(IRestService restService, IMvxNavigationService navigationService)
        {
            _restService = restService;
            _navigationService = navigationService;
        }

        public override void Prepare()
        {
            base.Prepare();
            RepositoriesLoadTask = MvxNotifyTask.Create(async () => await LoadRepositories());
        }

        private async Task LoadRepositories()
        {
            ResetPageCounters();
            var response = await GetRepositories(SearchPhrase, _page);
            if (response.IsSuccess)
            {
                Repositories = new MvxObservableCollection<Repository>(response.Results.Items);
                _totalCount = response.Results.TotalCount;
                HasMoreItems = _totalCount > Constants.RefitPerPage;
            }
            else
            {
                throw new Exception(response.FormattedErrorMessages);
            }
        }

        private async Task<Response<ItemsCollection<Repository>>> GetRepositories(string searchPhrase, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var perPage = Constants.RefitPerPage;
                var cacheKey = new QueryCacheKey {Page = page, SearchPhrase = searchPhrase}.ToString();
                var response =
                    await _restService.Execute<IGitHubApi, ItemsCollection<Repository>>(
                        api => api.GetRepositories(
                            cacheKey,
                            page,
                            perPage,
                            searchPhrase,
                            cancellationToken));
                if (response.IsSuccess && !response.Results.Items.Any())
                {
                    throw new Exception(Resources.Texts.NoResultsFound);
                }
                return response;
            }
            catch (Exception e)
            {
                return new Response<ItemsCollection<Repository>>().AddErrorMessage(e.Message).SetAsFailureResponse();
            }
        }

        private void ResetPageCounters()
        {
            HasMoreItems = false;
            LoadMoreTask = null;
            _page = 1;
        }

        private async Task LoadMoreItems()
        {
            _page++;
            var response = await GetRepositories(SearchPhrase, _page);
            if (response.IsSuccess)
            {
                Repositories.AddRange(response.Results.Items);
                _totalCount = response.Results.TotalCount;
                HasMoreItems = _page < 1000 / Constants.RefitPerPage && _totalCount > Constants.RefitPerPage * _page;
            }
            else
            {
                throw new Exception(response.FormattedErrorMessages);
            }
        }
    }
}
