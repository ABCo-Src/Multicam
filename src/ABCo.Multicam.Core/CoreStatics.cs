using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Interaction;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Data.Config;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Live.Types.ATEM;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Native;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Windows;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.UI.ViewModels.Features;
using System.Security.Cryptography;

namespace ABCo.Multicam.Core
{
	public static class CoreStatics
    {
        public static void Initialize(IParameteredServiceCollection container)
        {
			// Features
			container.AddSingleton<IMainFeatureCollection, MainFeatureCollection>();
			container.AddSingleton<IFeatureContentFactory, FeatureContentFactory>();
			container.AddSingleton<IScopedConnectionManager, ScopedConnectionManager>();
			container.AddSingleton<IScopedPresenterStoreFactory, ScopedPresenterStoreFactory>();
			container.AddTransient<IFeature, FeatureTypes, IFeatureDataSource, IFeatureActionTarget>((p1, p2, p3, s) => new Feature(p1, p2, p3, s));
			container.AddTransient<ILocallyInitializedFeatureDataSource, FeatureDataInfo[]>((p1, s) => new LocallyInitializedFeatureDataSource(p1));
			container.AddTransient<IUnsupportedLiveFeature, IInstantRetrievalDataSource>((p1, s) => new UnsupportedLiveFeature(p1));
            container.AddTransient<ISwitcherLiveFeature, IInstantRetrievalDataSource>(SwitcherLiveFeature.New);

			// Switcher
			container.AddTransient<ISwitcherFactory, SwitcherFactory>();
            container.AddTransient<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new HotSwappableSwitcherInteractionBuffer(p, s));
			container.AddTransient<IPerSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new PerSwitcherInteractionBuffer(p, s));
            container.AddTransient<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>((p1, p2, s) => new PerSpecSwitcherInteractionBuffer(p1, p2, s));
            container.AddSingleton<ISwitcherInteractionBufferFactory, SwitcherInteractionBufferFactory>();

            container.AddTransient<IDummySwitcher, DummySwitcherConfig>((p1, s) => new DummySwitcher(p1));
			container.AddSingleton<IATEMPlatformCompatibility, ATEMPlatformCompatibility>();
			container.AddTransient<IATEMSwitcher, ATEMSwitcherConfig>((p1, s) => new ATEMSwitcher(p1, s));
            container.AddTransient<IATEMConnection, ATEMSwitcherConfig, IATEMSwitcher>((p1, p2, s) => new ATEMConnection(p1, p2, s));
			container.AddTransient<IATEMCallbackHandler, IATEMSwitcher>((p1, s) => new ATEMCallbackHandler(p1));

#pragma warning disable
			container.AddSingleton<INativeATEMSwitcherDiscovery, WindowsNativeATEMSwitcherDiscovery>();
#pragma warning enable
		}
    }
}
