using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Tests;
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
        public static void Initialize(ServiceContainer container)
        {
            container.RegisterSingleton<MainWindowViewModel>();

            // Register binders
            container.RegisterTransient<IProjectFeaturesViewModel, ProjectFeaturesViewModel>();
            container.RegisterTransient<IBinderForProjectFeatures, ProjectFeaturesVMBinder>();
            container.RegisterTransient<IBinderForFeatureContainer, FeatureVMBinder>();
            container.RegisterTransient<IBinderForSwitcherFeature, SwitcherFeatureVMBinder>();
            container.RegisterTransient<IBinderForUnsupportedFeature, UnsupportedFeatureVMBinder>();

            // Register view-models
            container.RegisterTransient<IFeatureViewModel, FeatureViewModel>();
            container.RegisterTransient<ISwitcherFeatureVM, SwitcherFeatureViewModel>();

            CoreStatics.Initialize(container);
        }
    }
}
