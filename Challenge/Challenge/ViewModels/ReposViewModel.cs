using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Challenge.ViewModels
{
    public class ReposViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;

        public string Title => Resources.Texts.MainPageTitle;

        public IMvxCommand OpenPullRequestsCommand => new MvxAsyncCommand(async () => { await _navigationService.Navigate<PullRequestsViewModel>(); });

        public ReposViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
