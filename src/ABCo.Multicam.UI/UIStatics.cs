using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.UI
{
    public static class UIStatics
    {
        //public static string Log { get; set; }

        public static void Initialize(IServiceCollection container)
        {
            // Logging for debug purposes if you want it:
            //container.Initialize(registration => true,
            //    (f, i) => ServiceSource.N += $"Service requested: {i.GetType().Name}.   Thread: {Thread.CurrentThread.ManagedThreadId}\n"
            //);
            container.AddSingleton<MainWindowVM>();

            // Register binders
            container.AddTransient<IBinderForProjectFeatures, ProjectFeaturesVMBinder>();
            container.AddTransient<IBinderForFeatureContainer, FeatureVMBinder>();
            container.AddTransient<IBinderForSwitcherFeature, SwitcherFeatureVMBinder>();
            container.AddTransient<IBinderForSwitcherMixBlock, MixBlockVMBinder>();
            container.AddTransient<IBinderForUnsupportedFeature, UnsupportedFeatureVMBinder>();

            // Register view-models
            container.AddTransient<IApplicationVM, ApplicationVM>();
            container.AddTransient<IProjectVM, ProjectVM>();
            container.AddTransient<IProjectFeaturesVM, ProjectFeaturesVM>();
            container.AddTransient<IFeatureVM, FeatureVM>();
            container.AddTransient<ISwitcherFeatureVM, SwitcherFeatureVM>();
            container.AddTransient<ISwitcherConfigVM, SwitcherConfigVM>();
            container.AddTransient<ISwitcherMixBlockVM, SwitcherMixBlockVM>();
            container.AddTransient<ISwitcherCutButtonVM, SwitcherCutButtonVM>();
            container.AddTransient<ISwitcherAutoButtonVM, SwitcherAutoButtonVM>();
            container.AddTransient<ISwitcherProgramInputVM, SwitcherProgramInputVM>();
            container.AddTransient<ISwitcherPreviewInputVM, SwitcherPreviewInputVM>();
            container.AddTransient<ISwitcherConnectionVM, SwitcherConnectionVM>();

            container.AddSingleton<ISpecificSwitcherConfigVMFactory, SpecificSwitcherConfigVMFactory>();

            CoreStatics.Initialize(container);
        }
    }
}
