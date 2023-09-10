using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
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
            container.AddTransient<IProjectFeaturesPresenter, IFeatureManager>((p1, s) => new ProjectFeaturesPresenter(p1, s));
            container.AddTransient<IFeaturePresenter, IFeature, FeatureTypes>((p1, p2, s) => new FeaturePresenter(p1, p2, s));
			container.AddTransient<ISwitcherFeaturePresenter, IFeature>((p1, s) => new SwitcherFeaturePresenter(p1, s));
			container.AddTransient<ISwitcherConnectionPresenter, IFeature>((p1, s) => new SwitcherConnectionPresenter(p1, s));
			container.AddTransient<ISwitcherErrorPresenter, IFeature, Action>((p1, p2, s) => new SwitcherErrorPresenter(p1, p2, s));
			container.AddTransient<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IFeature>((p1, p2, s) => new SwitcherMixBlocksPresenter(p1, p2, s));

            // Register view-models
            container.AddTransient<IApplicationVM, ApplicationVM>();
            container.AddTransient<IProjectVM, ProjectVM>();
            container.AddTransient<IProjectFeaturesVM, IProjectFeaturesPresenterForVM>((p1, s) => new ProjectFeaturesVM(p1));
            container.AddTransient<IProjectFeaturesListItemVM, IProjectFeaturesPresenterForVM, IFeature, IFeatureVM>((p1, p2, p3, s) => new ProjectFeaturesListItemVM(p1, p2, p3));
            container.AddTransient<IFeatureVM, IFeaturePresenterForVM>((p1, s) => new FeatureVM(p1));
			container.AddTransient<ISwitcherFeatureVM, IFeature>((p1, s) => new SwitcherFeatureVM(p1));
            container.AddTransient<ISwitcherConfigVM, SwitcherConfig, ISwitcherFeatureVM>((p1, p2, s) => new SwitcherConfigVM(p1, p2, s));
            container.AddTransient<ISwitcherMixBlockVM, SwitcherMixBlockVM>();
            container.AddTransient<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>((p1, p2, s) => new SwitcherCutButtonVM(p1, p2));
            container.AddTransient<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherProgramInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherPreviewInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherConnectionVM, ISwitcherErrorPresenter>((p1, s) => new SwitcherConnectionVM(p1));

            container.AddSingleton<ISpecificSwitcherConfigVMFactory, SpecificSwitcherConfigVMFactory>();

            CoreStatics.Initialize(container);
        }
    }
}
