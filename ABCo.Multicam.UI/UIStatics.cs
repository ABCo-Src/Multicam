using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI
{
    public static class UIStatics
    {
        //public static string Log { get; set; }

        public static void Initialize(ServiceContainer container)
        {
            // Logging for debug purposes if you want it:
            //container.Initialize(registration => true,
            //    (f, i) => ServiceSource.N += $"Service requested: {i.GetType().Name}.   Thread: {Thread.CurrentThread.ManagedThreadId}\n"
            //);
            container.RegisterSingleton<MainWindowViewModel>();

            // Register binders
            container.RegisterTransient<IApplicationViewModel, ApplicationViewModel>();
            container.RegisterTransient<IProjectViewModel, ProjectViewModel>();
            container.RegisterTransient<IProjectFeaturesViewModel, ProjectFeaturesViewModel>();
            container.RegisterTransient<IBinderForProjectFeatures, ProjectFeaturesVMBinder>();
            container.RegisterTransient<IBinderForFeatureContainer, FeatureVMBinder>();
            container.RegisterTransient<IBinderForSwitcherFeature, SwitcherFeatureVMBinder>();
            container.RegisterTransient<IBinderForSwitcherMixBlock, MixBlockVMBinder>();
            container.RegisterTransient<IBinderForUnsupportedFeature, UnsupportedFeatureVMBinder>();

            // Register view-models
            container.RegisterTransient<IFeatureViewModel, FeatureViewModel>();
            container.RegisterTransient<ISwitcherFeatureVM, SwitcherFeatureViewModel>();
            container.RegisterTransient<ISwitcherMixBlockVM, SwitcherMixBlockViewModel>();
            container.RegisterTransient<ISwitcherCutButtonViewModel, SwitcherCutButtonViewModel>();
            container.RegisterTransient<ISwitcherAutoButtonViewModel, SwitcherAutoButtonViewModel>();
            container.RegisterTransient<ISwitcherProgramInputViewModel, SwitcherProgramInputViewModel>();
            container.RegisterTransient<ISwitcherPreviewInputViewModel, SwitcherPreviewInputViewModel>();

            CoreStatics.Initialize(container);
        }
    }
}
