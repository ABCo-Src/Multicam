using ABCo.Multicam.UI.Avalonia.Services;
using ABCo.Multicam.UI.Services;
using Avalonia.Controls.ApplicationLifetimes;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Avalonia
{
    public static class UIAvaloniaStatics
    {
        //public static ServiceContainer StaticContainer { get; private set; }

        public static ServiceContainer InitializeContainer(IApplicationLifetime lifetime)
        {
            var container = new ServiceContainer();
            //StaticContainer = container;

            // Register capabilities
            if (lifetime is IClassicDesktopStyleApplicationLifetime)
                container.RegisterSingleton<IUIPlatformWindowCapabilities, DesktopUIPlatformWindowCapabilities>();
            else if (lifetime is ISingleViewApplicationLifetime)
                container.RegisterSingleton<IUIPlatformWindowCapabilities, WebUIPlatformWindowCapabilities>();
            else throw new Exception("Window capabilities weren't added for new lifetime.");

            // Register the next layer down now
            UIStatics.Initialize(container);

            return container;
        }
    }
}
