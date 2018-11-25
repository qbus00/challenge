using Foundation;
using MvvmCross.Forms.Platforms.Ios.Core;
using UIKit;

namespace Challenge.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxFormsApplicationDelegate<MvxFormsIosSetup<MvxApp, App>, MvxApp, App>
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}