using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using LightInject;

namespace ABCo.Multicam.Core
{
    public static class CoreStatics
    {
        public static void Initialize(ServiceContainer container)
        {
            container.RegisterInstance<IServiceSource>(new ServiceSource(container));

            // Features
            container.RegisterSingleton<IFeatureManager, FeatureManager>();
            container.RegisterTransient<IFeatureContainer, FeatureContainer>();
            container.RegisterTransient<IUnsupportedRunningFeature, UnsupportedRunningFeature>();
            container.RegisterTransient<ISwitcherRunningFeature, SwitcherRunningFeature>();

            // Switcher
            container.RegisterTransient<ISwitcherFactory, SwitcherFactory>();
            container.RegisterTransient<IDynamicSwitcherInteractionBuffer, DynamicSwitcherInteractionBuffer>();
            container.RegisterTransient<IPerSpecSwitcherInteractionBuffer, PerSpecSwitcherInteractionBuffer>();
            container.RegisterSingleton<ISwitcherInteractionBufferFactory, SwitcherInteractionBufferFactory>();
            container.RegisterTransient<IDummySwitcher, DummySwitcher>();
        }
    }
}
