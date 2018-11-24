using Challenge.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Challenge.Pages
{
    [MvxMasterDetailPagePresentation]

    public partial class PullRequestsPage : MvxContentPage<PullRequestsViewModel>
	{
		public PullRequestsPage ()
		{
			InitializeComponent ();
        }
    }
}