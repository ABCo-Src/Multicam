using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.Presenters.Features.Switcher;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;

namespace ABCo.Multicam.UI
{
	public static class UIStatics
    {
        //public static string Log { get; set; }

        public static void Initialize(IParameteredServiceCollection container)
        {
            // Logging for debug purposes if you want it:
            //container.Initialize(registration => true,
            //    (f, i) => ServiceSource.N += $"Service requested: {i.GetType().Name}.   Thread: {Thread.CurrentThread.ManagedThreadId}\n"
            //);
            container.AddSingleton<IMainWindowVM, MainWindowVM>();

            // Register presenters
            container.AddTransient<IProjectFeaturesPresenter, IFeatureManager>(ProjectFeaturesPresenter.New);
            container.AddTransient<IFeaturePresenter, IFeature, FeatureTypes>(FeaturePresenter.New);
			container.AddTransient<ISwitcherFeaturePresenter, IFeature>(SwitcherFeaturePresenter.New);
			container.AddTransient<ISwitcherConnectionPresenter, IFeature>(SwitcherConnectionPresenter.New);
			container.AddTransient<ISwitcherErrorPresenter, IFeature, Action>(SwitcherErrorPresenter.New);
			container.AddTransient<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IFeature>(SwitcherMixBlocksPresenter.New);

            // Register view-models
            container.AddTransient<IApplicationVM, ApplicationVM>();
            container.AddTransient<IProjectVM, ProjectVM>();
            container.AddTransient<IProjectFeaturesVM, IProjectFeaturesPresenterForVM>(ProjectFeaturesVM.New);
            container.AddTransient<IProjectFeaturesListItemVM, IProjectFeaturesPresenterForVM, IFeature, IFeatureVM>((param1, param2, param3, s) => new ProjectFeaturesListItemVM(param1, param2, param3));
            container.AddTransient<IFeatureVM, IFeaturePresenterForVM>(FeatureVM.New);
			container.AddTransient<ISwitcherFeatureVM, IFeature>(SwitcherFeatureVM.New);
            container.AddTransient<ISwitcherConfigVM, SwitcherConfig, ISwitcherFeatureVM>(SwitcherConfigVM.New);
            container.AddTransient<ISwitcherMixBlockVM, SwitcherMixBlockVM>();
            container.AddTransient<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>(SwitcherCutButtonVM.New);
            container.AddTransient<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>(SwitcherProgramInputVM.New);
			container.AddTransient<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>(SwitcherPreviewInputVM.New);
			container.AddTransient<ISwitcherConnectionVM, ISwitcherErrorPresenter>(SwitcherConnectionVM.New);

            container.AddSingleton<ISpecificSwitcherConfigVMFactory, SpecificSwitcherConfigVMFactory>();

            CoreStatics.Initialize(container);
        }
    }
}
