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
using ABCo.Multicam.Core.General;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Core.Hosting.Server;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Scoping;
using ABCo.Multicam.UI.ViewModels.Features;
using System.Security.Cryptography;

namespace ABCo.Multicam.Core
{
	public interface IServerConnection
	{
		IPlatformInfo GetPlatformInfo();
		IServerTarget GetFeatures();
	}

	public class LocalMulticamServer : IServerConnection
    {
		public IServerInfo ServerInfo { get; }

		public LocalMulticamServer(Func<IServerInfo, IPlatformInfo> getPlatformInfo, Func<IServerInfo, IActiveServerHost> getServerHost, IThreadDispatcher dispatcher)
		{
			// Initialize our services
			ServerInfo = InitializeServices(getPlatformInfo, getServerHost, dispatcher);
		}

        public IServerInfo InitializeServices(Func<IServerInfo, IPlatformInfo> getPlatformInfo, Func<IServerInfo, IActiveServerHost> getServerHost, IThreadDispatcher dispatcher)
        {
			var container = new ServerInfo(dispatcher, this); // NOTE: For now, this is really a singleton internally

			// Hosting/general
			container.AddSingleton(getPlatformInfo);
			container.AddSingleton(getServerHost);
			container.AddSingleton<IConnectedClientsManager>(s => new ConnectedClientsManager(s));
			container.AddTransient<IConnectedClientsBoundClientNotifier, IServerTarget>((p1, s) => new ClientNotifier(p1, s));
			container.AddTransient<IDispatchingServerTarget, IServerTarget>((p1, s) => new DispatchingServerTarget(p1, s));

			// Features
			container.AddSingleton<IMainFeatureCollection>(s => new MainFeatureCollection(s));
			container.AddSingleton<IFeatureContentFactory>(s => new FeatureContentFactory(s));
			container.AddTransient<IFeature, FeatureTypes, IFeatureDataSource, IFeatureActionTarget>((p1, p2, p3, s) => new Feature(p1, p2, p3, s));
			container.AddTransient<ILocallyInitializedFeatureDataSource, FeatureDataInfo[]>((p1, s) => new LocallyInitializedFeatureDataSource(p1));
			container.AddTransient<IUnsupportedLiveFeature, IInstantRetrievalDataSource>((p1, s) => new UnsupportedLiveFeature(p1));
            container.AddTransient<ISwitcherLiveFeature, IInstantRetrievalDataSource>(SwitcherLiveFeature.New);

			// Switcher
			container.AddTransient<ISwitcherFactory>(s => new SwitcherFactory(s));
            container.AddTransient<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new HotSwappableSwitcherInteractionBuffer(p, s));
			container.AddTransient<IPerSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new PerSwitcherInteractionBuffer(p, s));
            container.AddTransient<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>((p1, p2, s) => new PerSpecSwitcherInteractionBuffer(p1, p2, s));
            container.AddSingleton<ISwitcherInteractionBufferFactory>(s => new SwitcherInteractionBufferFactory());

            container.AddTransient<IDummySwitcher, DummySwitcherConfig>((p1, s) => new DummySwitcher(p1));
			container.AddSingleton<IATEMPlatformCompatibility>(s => new ATEMPlatformCompatibility(s));
			container.AddTransient<IATEMSwitcher, ATEMSwitcherConfig>((p1, s) => new ATEMSwitcher(p1, s));
            container.AddTransient<IATEMConnection, ATEMSwitcherConfig, IATEMSwitcher>((p1, p2, s) => new ATEMConnection(p1, p2, s));
			container.AddTransient<IATEMCallbackHandler, IATEMSwitcher>((p1, s) => new ATEMCallbackHandler(p1));

#pragma warning disable
			container.AddSingleton<INativeATEMSwitcherDiscovery>(s => new WindowsNativeATEMSwitcherDiscovery());
#pragma warning enable

			return container;
		}

		public IServerTarget GetFeatures() => ServerInfo.Get<IMainFeatureCollection>();
		public IPlatformInfo GetPlatformInfo() => ServerInfo.Get<IPlatformInfo>();
	}
}
