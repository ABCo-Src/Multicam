using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Presenters.Features.Switcher;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Microsoft.Extensions.DependencyInjection;

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

            // Register binders
            container.AddTransient<IBinderForProjectFeatures, ProjectFeaturesVMBinder>();

            // Register view-models
            container.AddTransient<IApplicationVM, ApplicationVM>();
            container.AddTransient<IProjectVM, ProjectVM>();
            container.AddTransient<IProjectFeaturesVM, ProjectFeaturesVM>();
            container.AddTransient<IFeatureVM, FeatureVM>();
			container.AddTransient<ISwitcherFeatureVM, IFeature>(SwitcherFeatureVM.New);
            container.AddTransient<ISwitcherConfigVM, SwitcherConfig, ISwitcherFeatureVM>(SwitcherConfigVM.New);
            container.AddTransient<ISwitcherMixBlockVM, SwitcherMixBlockVM>();
            container.AddTransient<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>(SwitcherCutButtonVM.New);
            container.AddTransient<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>(SwitcherProgramInputVM.New);
			container.AddTransient<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>(SwitcherPreviewInputVM.New);
			container.AddTransient<ISwitcherConnectionVM, ISwitcherRunningFeature>(SwitcherConnectionVM.New);

            container.AddSingleton<ISpecificSwitcherConfigVMFactory, SpecificSwitcherConfigVMFactory>();

            CoreStatics.Initialize(container);
        }
    }
}
