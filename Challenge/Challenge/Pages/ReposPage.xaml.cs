using System;
using System.ComponentModel;
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

        private void RepoListView_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RepoListView.ItemsSource))
            {
                if (ViewModel?.Repositories?.Count > 0)
                {
                    RepoListView.ScrollTo(ViewModel.Repositories[0], ScrollToPosition.Start, false);
                }
            }
        }
    }
}