using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management;
using ABCo.Multicam.Server.Features.Switchers.Buffering;
using ABCo.Multicam.Server.Features.Switchers.Core;
using ABCo.Multicam.Server.Features.Switchers.Core.ATEM;
using ABCo.Multicam.Server.Features.Switchers.Core.ATEM.Native;

namespace ABCo.Multicam.Server
{
	public interface IServerConnection
	{
		IPlatformInfo GetPlatformInfo();
		IMainFeatureCollection GetFeatures();
		IHostingManager GetHostingManager();
		void Disconnect(IClientInfo info);
	}

	public class LocalMulticamServer : IServerConnection
    {
		public IServerInfo ServerInfo { get; }

		public LocalMulticamServer(Func<IServerInfo, IPlatformInfo> getPlatformInfo, Func<NativeServerHostConfig, IServerInfo, INativeServerHost> getServerHost, Func<Action, IServerInfo, ILocalIPCollection> getAvaIP, IThreadDispatcher dispatcher)
		{
			// Initialize our services
			ServerInfo = InitializeServices(getPlatformInfo, getServerHost, getAvaIP, dispatcher);
		}

        public IServerInfo InitializeServices(Func<IServerInfo, IPlatformInfo> getPlatformInfo, Func<NativeServerHostConfig, IServerInfo, INativeServerHost> getServerHost, Func<Action, IServerInfo, ILocalIPCollection> getAvaIP, IThreadDispatcher dispatcher)
        {
			var container = new ServerInfo(dispatcher, this); // NOTE: For now, this is really a singleton internally

			// Hosting/general
			Server.ServerInfo.AddSingleton(getPlatformInfo);
			Server.ServerInfo.AddTransient(getServerHost);
			Server.ServerInfo.AddTransient(getAvaIP);
			Server.ServerInfo.AddSingleton<IConnectedClientsManager>(s => new ConnectedClientsManager(s));
			Server.ServerInfo.AddSingleton<IHostingManager>(s => new HostingManager(s));

			// Features
			Server.ServerInfo.AddSingleton<IMainFeatureCollection>(s => new MainFeatureCollection(s));
			Server.ServerInfo.AddSingleton<IFeatureContentFactory>(s => new FeatureContentFactory(s));
			Server.ServerInfo.AddTransient<IUnsupportedLiveFeature>((s) => new UnsupportedLiveFeature(s));
			Server.ServerInfo.AddTransient<ISwitcherFeature>((s) => new SwitcherFeature(s));

			// Switcher
			Server.ServerInfo.AddTransient<ISwitcherFactory>(s => new SwitcherFactory(s));
			Server.ServerInfo.AddTransient<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new HotSwappableSwitcherInteractionBuffer(p, s));
			Server.ServerInfo.AddTransient<IPerSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new PerSwitcherInteractionBuffer(p, s));
			Server.ServerInfo.AddTransient<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>((p1, p2, s) => new PerSpecSwitcherInteractionBuffer(p1, p2, s));
			Server.ServerInfo.AddSingleton<ISwitcherInteractionBufferFactory>(s => new SwitcherInteractionBufferFactory());

			Server.ServerInfo.AddTransient<IDummySwitcher, DummySwitcherConfig>((p1, s) => new DummySwitcher(p1));
			Server.ServerInfo.AddSingleton<IATEMPlatformCompatibility>(s => new ATEMPlatformCompatibility(s));
			Server.ServerInfo.AddTransient<IATEMSwitcher, ATEMSwitcherConfig>((p1, s) => new ATEMSwitcher(p1, s));
			Server.ServerInfo.AddTransient<IATEMConnection, ATEMSwitcherConfig, IATEMSwitcher>((p1, p2, s) => new ATEMConnection(p1, p2, s));
			Server.ServerInfo.AddTransient<IATEMCallbackHandler, IATEMSwitcher>((p1, s) => new ATEMCallbackHandler(p1));

#pragma warning disable
			Server.ServerInfo.AddSingleton<INativeATEMSwitcherDiscovery>(s => new WindowsNativeATEMSwitcherDiscovery());
#pragma warning enable

			return container;
		}

		public IMainFeatureCollection GetFeatures() => ServerInfo.Get<IMainFeatureCollection>();
		public IHostingManager GetHostingManager() => ServerInfo.Get<IHostingManager>();
		public IPlatformInfo GetPlatformInfo() => ServerInfo.Get<IPlatformInfo>();
		public void Disconnect(IClientInfo info) => ServerInfo.Get<IConnectedClientsManager>().OnClientDisconnected(info);
	}
}
