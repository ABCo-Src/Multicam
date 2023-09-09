using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Native;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.Core
{
	public static class CoreStatics
    {
        public static void Initialize(IParameteredServiceCollection container)
        {
			// Features
			container.AddSingleton<IFeatureManager, FeatureManager>();
			container.AddTransient<IFeature, FeatureTypes>(Feature.New);
			container.AddTransient<IUnsupportedRunningFeature, UnsupportedRunningFeature>();
            container.AddTransient<ISwitcherRunningFeature, SwitcherLiveFeature>();

            // Switcher
            container.AddTransient<ISwitcherFactory, SwitcherFactory>();
            container.AddTransient<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>(HotSwappableSwitcherInteractionBuffer.New);
			container.AddTransient<IPerSwitcherInteractionBuffer, SwitcherConfig>(PerSwitcherInteractionBuffer.New);
            container.AddTransient<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(PerSpecSwitcherInteractionBuffer.New);
            container.AddSingleton<ISwitcherInteractionBufferFactory, SwitcherInteractionBufferFactory>();

            container.AddTransient<IDummySwitcher, DummySwitcherConfig>(DummySwitcher.New);
			container.AddTransient<IATEMSwitcher, ATEMSwitcherConfig>(ATEMSwitcher.New);
            container.AddTransient<IATEMConnection, ISwitcher>(ATEMConnection.New);
			container.AddTransient<IATEMCallbackHandler, ISwitcher>(ATEMCallbackHandler.New);

#pragma warning disable
			container.AddSingleton<INativeATEMSwitcherDiscovery, WindowsNativeATEMSwitcherDiscovery>();
#pragma warning enable
		}
    }
}
