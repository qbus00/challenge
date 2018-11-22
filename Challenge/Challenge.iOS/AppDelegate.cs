using Foundation;
using MvvmCross.Forms.Platforms.Ios.Core;

namespace Challenge.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxFormsApplicationDelegate<MvxFormsIosSetup<MvxApp, App>, MvxApp, App>
    {

    }
}