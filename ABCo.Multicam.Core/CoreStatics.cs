using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Switchers;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core
{
    public static class CoreStatics
    {
        public static void Initialize(ServiceContainer container)
        {
            container.RegisterInstance<IServiceSource>(new ServiceSource(container));

            // Strips
            container.RegisterSingleton<IStripManager, StripManager>();
            container.RegisterSingleton<IUnsupportedRunningStrip, UnsupportedRunningStrip>();
            container.RegisterSingleton<ISwitcherRunningStrip, SwitcherRunningStrip>();
        }
    }
}
