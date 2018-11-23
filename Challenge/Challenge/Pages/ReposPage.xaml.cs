using System.Reactive.Linq;
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
    }
}