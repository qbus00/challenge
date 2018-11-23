using System;
using System.Threading.Tasks;
using Challenge.IncrementalLoading;
using Challenge.Model;
using Challenge.Rest;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Refit.Insane.PowerPack.Services;

namespace Challenge.ViewModels
{
    public class ReposViewModel : MvxViewModel, ISupportIncrementalLoading
    {
        private readonly IRestService _restService;
        private readonly IMvxNavigationService _navigationService;

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
            HasMoreItems = false;
            LoadMoreTask = null;
            _page = 1;
            var response =
                await _restService.Execute<IGitHubApi, ItemsCollection<Repository>>(api => api.GetRepositories(_page, default(System.Threading.CancellationToken)));
            if (response.IsSuccess)
            {
                Repositories = new MvxObservableCollection<Repository>(response.Results.Items);
                _totalCount = response.Results.TotalCount;
                HasMoreItems = _totalCount > 20;
            }
            else
            {
                throw new Exception(response.FormattedErrorMessages);
            }
        }

        public int PageSize { get; set; } = 20;

        private bool _hasMoreItems;
        public bool HasMoreItems
        {
            get => _hasMoreItems;
            set => SetProperty(ref _hasMoreItems, value);
        }

        public IMvxCommand LoadMoreItemsCommand => new MvxCommand(
            ()=> { LoadMoreTask = MvxNotifyTask.Create(async () => await LoadMoreItems()); });

        private async Task LoadMoreItems()
        {
            _page++;
            var response =
                await _restService.Execute<IGitHubApi, ItemsCollection<Repository>>(api => api.GetRepositories(_page, default(System.Threading.CancellationToken)));
            if (response.IsSuccess)
            {
                Repositories.AddRange(response.Results.Items);
                _totalCount = response.Results.TotalCount;
                HasMoreItems = _page < 50 && _totalCount > 20 * _page;
            }
            else
            {
                throw new Exception(response.FormattedErrorMessages);
            }
        }
    }
}
