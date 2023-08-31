using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.Core
{
    public static class CoreStatics
    {
        public static void Initialize(IServiceCollection container)
        {
			// Features
			container.AddSingleton<IFeatureManager, FeatureManager>();
			container.AddTransient<IFeatureContainer, FeatureContainer>();
			container.AddTransient<IUnsupportedRunningFeature, UnsupportedRunningFeature>();
            container.AddTransient<ISwitcherRunningFeature, SwitcherRunningFeature>();

            // Switcher
            container.AddTransient<ISwitcherFactory, SwitcherFactory>();
            container.AddTransient<IDynamicSwitcherInteractionBuffer, DynamicSwitcherInteractionBuffer>();
            container.AddTransient<IPerSpecSwitcherInteractionBuffer, PerSpecSwitcherInteractionBuffer>();
            container.AddSingleton<ISwitcherInteractionBufferFactory, SwitcherInteractionBufferFactory>();
            container.AddTransient<IDummySwitcher, DummySwitcher>();
        }
    }
}
