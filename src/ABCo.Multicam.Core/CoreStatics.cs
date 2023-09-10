using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Native;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Windows;
using ABCo.Multicam.UI.ViewModels.Features;
using System.Security.Cryptography;

namespace ABCo.Multicam.Core
{
	public static class CoreStatics
    {
        public static void Initialize(IParameteredServiceCollection container)
        {
			// Features
			container.AddSingleton<IFeatureManager, FeatureManager>();
			container.AddSingleton<IFeatureContentFactory, FeatureContentFactory>();
			container.AddTransient<IFeature, FeatureTypes>(Feature.New);
			container.AddTransient<ILocalFeatureInteractionHandler, FeatureTypes, FeatureDataInfo[]>((p1, p2, s) => new LocalFeatureInteractionHandler(p1, p2, s));
			container.AddTransient<IUnsupportedLiveFeature, UnsupportedLiveFeature>();
            container.AddTransient<ISwitcherLiveFeature, ILocalFragmentCollection>(SwitcherLiveFeature.New);

			// Switcher
			container.AddTransient<ISwitcherFactory, SwitcherFactory>();
            container.AddTransient<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new HotSwappableSwitcherInteractionBuffer(p, s));
			container.AddTransient<IPerSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new PerSwitcherInteractionBuffer(p, s));
            container.AddTransient<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>((p1, p2, s) => new PerSpecSwitcherInteractionBuffer(p1, p2, s));
            container.AddSingleton<ISwitcherInteractionBufferFactory, SwitcherInteractionBufferFactory>();

            container.AddTransient<IDummySwitcher, DummySwitcherConfig>((p1, s) => new DummySwitcher(p1));
			container.AddTransient<IATEMSwitcher, ATEMSwitcherConfig>((p1, s) => new ATEMSwitcher(p1, s));
            container.AddTransient<IATEMConnection, ISwitcher>((p1, s) => new ATEMConnection(p1, s));
			container.AddTransient<IATEMCallbackHandler, ISwitcher>((p1, s) => new ATEMCallbackHandler(p1));

#pragma warning disable
			container.AddSingleton<INativeATEMSwitcherDiscovery, WindowsNativeATEMSwitcherDiscovery>();
#pragma warning enable
		}
    }
}
