using ABCo.Multicam.Core;
using ABCo.Multicam.Tests.UI;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Strips;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
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
            container.Register<StripViewModelInfo, ISwitcherStripViewModel>((factory, info) => new SwitcherStripViewModel(info, factory.GetInstance<IServiceSource>()));

            CoreStatics.Initialize(container);
        }
    }
}
