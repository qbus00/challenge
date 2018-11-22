using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Forms.Platforms.Android.Views;

namespace Challenge.Droid
{
    [Activity(
        Label = "Challenge"
        , MainLauncher = true
        , Icon = "@mipmap/icon"
        , Theme = "@style/SplashTheme"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxFormsSplashScreenActivity<Setup, MvxApp, App>
    {
        public SplashScreen()
            : base(Resource.Layout.SplashLayout)
        {
        }

        protected override Task RunAppStartAsync(Bundle bundle)
        {
            StartActivity(typeof(MainActivity));
            return Task.CompletedTask;
        }
    }
}