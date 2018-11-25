using System;
using System.Reflection;
using Challenge.ViewModels;
using MvvmCross;
using MvvmCross.ViewModels;
using Refit.Insane.PowerPack.Caching;
using Refit.Insane.PowerPack.Caching.Internal;
using Refit.Insane.PowerPack.Configuration;
using Refit.Insane.PowerPack.Services;

namespace Challenge
{
    public class MvxApp : MvxApplication
    {
        public override void Initialize()
        {
            BaseApiConfiguration.ApiUri = new Uri("https://api.github.com");
            Mvx.IoCProvider.RegisterType<IPersistedCache, AkavachePersistedCache>();
            Mvx.IoCProvider.RegisterType(() =>
            {
                var restServiceBuilder = new RestServiceBuilder();
                return restServiceBuilder.BuildRestService(typeof(MvxApp).GetTypeInfo().Assembly);
            });
            RegisterAppStart<RootViewModel>();
        }
    }
}
