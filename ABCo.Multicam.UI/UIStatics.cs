using ABCo.Multicam.Core;
using ABCo.Multicam.Tests;
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
            // Register view-models
            container.RegisterSingleton<MainWindowViewModel>();
            //container.RegisterSingleton<IApplicationViewModel, ApplicationViewModel>();
            //container.RegisterSingleton<IProjectViewModel, ProjectViewModel>();
            //container.RegisterSingleton<IProjectStripsViewModel, ProjectStripsViewModel>();
            //container.RegisterSingleton<IStripViewModel, StripViewModel>();
            container.Register<NewViewModelInfo, ISwitcherFeatureVM>((factory, info) => new SwitcherFeatureViewModel(info, factory.GetInstance<IServiceSource>()));
            container.Register<NewViewModelInfo, ISwitcherMixBlockVM>((factory, info) => new SwitcherMixBlockViewModel(info, factory.GetInstance<IServiceSource>()));
            container.Register<NewViewModelInfo, ISwitcherPreviewInputViewModel>((factory, info) => new SwitcherPreviewInputViewModel(info, factory.GetInstance<IServiceSource>()));
            container.Register<NewViewModelInfo, ISwitcherProgramInputViewModel>((factory, info) => new SwitcherProgramInputViewModel(info, factory.GetInstance<IServiceSource>()));
            container.Register<NewViewModelInfo, ISwitcherCutButtonViewModel>((factory, info) => new SwitcherCutButtonViewModel(info, factory.GetInstance<IServiceSource>()));
            container.Register<NewViewModelInfo, ISwitcherAutoButtonViewModel>((factory, info) => new SwitcherAutoButtonViewModel(info, factory.GetInstance<IServiceSource>()));

            CoreStatics.Initialize(container);
        }
    }
}
