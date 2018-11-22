using Challenge.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Challenge.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Root, WrapInNavigationPage = false)]
    public partial class RootPage : MvxMasterDetailPage<RootViewModel>
    {
        public RootPage()
        {
            InitializeComponent();
        }
    }
}