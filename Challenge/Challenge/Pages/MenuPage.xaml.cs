using Challenge.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Challenge.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Master)]
    public partial class MenuPage : MvxContentPage<MenuViewModel>
	{
		public MenuPage ()
		{
			InitializeComponent ();
		}
	}
}