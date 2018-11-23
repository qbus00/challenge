using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Challenge.IncrementalLoading;
using Challenge.Model;
using Challenge.Rest;
using Challenge.Threading;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Refit.Insane.PowerPack.Data;
using Refit.Insane.PowerPack.Services;

namespace Challenge.ViewModels
{
    public class ReposViewModel : MvxViewModel, ISupportIncrementalLoading
    {
        private readonly IRestService _restService;
        private readonly IMvxNavigationService _navigationService;
        private IDisposable _searchObservable;

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


        AsyncAutoResetEvent _isSearching = new AsyncAutoResetEvent();
        private void SetupSearch()
        {
            _searchObservable = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler)
                .ObserveOn(SynchronizationContext.Current)
                .Where(pattern => pattern.EventArgs.PropertyName == nameof(SearchPhrase))
                .Select(pattern => SearchPhrase.Trim(' ', '\t'))
                .Throttle(TimeSpan.FromMilliseconds(300))
                .DistinctUntilChanged()
                .Select(searchPhrase => Observable.FromAsync(async token =>
                {
                    RepositoriesLoadTask = MvxNotifyTask.Create(() =>
                    {
                        if (RepositoriesLoadTask.IsNotCompleted)
                        {
                            _isSearching?.Set();
                        }
                        _isSearching = new AsyncAutoResetEvent();
                        return _isSearching.WaitAsync();
                    });
                    var result = await GetRepositories(searchPhrase, 1, token);
                    return result;
                })).Switch().Subscribe(response =>
                {
                    _isSearching?.Set();
                    _isSearching = null;
                    if (response.IsSuccess)
                    {
                        ResetPageCounters();
                        Repositories = new MvxObservableCollection<Repository>(response.Results.Items);
                        _totalCount = response.Results.TotalCount;
                        HasMoreItems = _totalCount > Constants.RefitPerPage;
                    }
                });
        }

        public IMvxCommand LoadMoreItemsCommand => new MvxCommand(
            () => { LoadMoreTask = MvxNotifyTask.Create(async () => await LoadMoreItems()); });

        public string Title => Resources.Texts.MainPageTitle;

        public ReposViewModel(IMvxNavigationService navigationService, IRestService restService)
        {
            _navigationService = navigationService;
            _restService = restService;
        }

        public override void Prepare()
        {
            base.Prepare();
            RepositoriesLoadTask = MvxNotifyTask.Create(async () => await LoadRepositories());
        }

        private async Task LoadRepositories()
        {
            ResetPageCounters();
            var response = await GetRepositories(string.Empty, _page);
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

        private async Task<Response<ItemsCollection<Repository>>> GetRepositories(string searchPhrase, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cacheKey = new QueryCacheKey {Page = page, SearchPhrase = searchPhrase}.ToString();
            var response =
                await _restService.Execute<IGitHubApi, ItemsCollection<Repository>>(
                    api => api.GetRepositories(
                        cacheKey,
                        page,
                        searchPhrase,
                        cancellationToken));
            return response;
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
