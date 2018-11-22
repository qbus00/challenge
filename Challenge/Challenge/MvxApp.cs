using Challenge.ViewModels;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace Challenge
{
    public class MvxApp : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            RegisterAppStart<ReposViewModel>();
        }
    }
}
