using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Windows;
using ABCo.Multicam.Core.General;
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
            container.AddTransient<IHotSwappableSwitcherInteractionBuffer, HotSwappableSwitcherInteractionBuffer>();
            container.AddTransient<IPerSwitcherInteractionBuffer, PerSwitcherInteractionBuffer>();
            container.AddTransient<IPerSpecSwitcherInteractionBuffer, PerSpecSwitcherInteractionBuffer>();
            container.AddSingleton<ISwitcherInteractionBufferFactory, SwitcherInteractionBufferFactory>();
            container.AddTransient<IDummySwitcher, DummySwitcher>();
            container.AddTransient<IATEMSwitcher, ATEMSwitcher>();
            container.AddTransient<IATEMConnection, ATEMConnection>();
			container.AddTransient<IATEMCallbackHandler, ATEMCallbackHandler>();

#pragma warning disable
            container.AddSingleton<IATEMRawAPI, WindowsATEMRawAPI>();
#pragma warning enable
		}
    }
}
