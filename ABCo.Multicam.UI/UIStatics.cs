using ABCo.Multicam.UI.ViewModels;
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
            container.RegisterSingleton<IApplicationViewModel, ApplicationViewModel>();
            container.RegisterSingleton<IProjectViewModel, ProjectViewModel>();
        }
    }
}
