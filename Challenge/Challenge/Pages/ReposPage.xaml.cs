using System.ComponentModel;
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
            RepoListView.PreloadCount = Constants.RefitPreloadPerPage;

            SearchBar.Placeholder = Challenge.Resources.Texts.SearchPlaceholder;
        }

        private void RepoListView_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RepoListView.ItemsSource))
            {
                if (ViewModel?.Repositories?.Count > 0)
                {
                    RepoListView.ScrollTo(ViewModel.Repositories[0],
                        Device.RuntimePlatform == Device.iOS ? ScrollToPosition.End : ScrollToPosition.Start, 
                        Device.RuntimePlatform == Device.iOS);
                }
            }
        }
    }
}