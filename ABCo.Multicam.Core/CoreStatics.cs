using ABCo.Multicam.Core.Strips;
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
            container.RegisterSingleton<IStripManager, StripManager>();
        }
    }
}
