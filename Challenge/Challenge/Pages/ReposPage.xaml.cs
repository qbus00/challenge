using System;
using System.Threading.Tasks;
using Challenge.Model;
using Challenge.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Challenge.Pages
{
    [MvxMasterDetailPagePresentation(NoHistory = true)]
    public partial class ReposPage : MvxContentPage<ReposViewModel>
    {
        public ReposPage()
        {
            InitializeComponent();
            SearchBar.Placeholder = Challenge.Resources.Texts.SearchPlaceholder;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<Repository>(this, Constants.ScrollToTopMessage, repository =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    RepoListView.ScrollTo(repository, ScrollToPosition.Start, false);
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object>(this, Constants.ScrollToTopMessage);
        }
    }
}