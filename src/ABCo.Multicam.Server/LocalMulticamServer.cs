using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Management;
using ABCo.Multicam.Server.Features.Switchers.Buffering;
using ABCo.Multicam.Server.Features.Switchers.Core;
using ABCo.Multicam.Server.Features.Switchers.Core.ATEM;
using ABCo.Multicam.Server.Features.Switchers.Core.ATEM.Native;
using ABCo.Multicam.Server.General.Factories;
using ABCo.Multicam.Server.Automation.Buttons;

namespace ABCo.Multicam.Server
{
    public interface IMulticamServer
	{
		public IThreadDispatcher Dispatcher { get; }
		IPlatformInfo GetPlatformInfo();
		ISwitcherList GetFeatures();
		IScriptButtonList GetAutoButtons();
		IHostingManager GetHostingManager();
	}

	public class LocalMulticamServer : IMulticamServer
    {
		public IThreadDispatcher Dispatcher { get; }
		public IServerInfo ServerInfo { get; }

		public LocalMulticamServer(Func<IServerInfo, IPlatformInfo> getPlatformInfo, Func<NativeServerHostConfig, IServerInfo, INativeServerHost> getServerHost, Func<Action, IServerInfo, ILocalIPCollection> getAvaIP, IThreadDispatcher dispatcher)
		{
			// Initialize our services
			Dispatcher = dispatcher;
			ServerInfo = InitializeServices(getPlatformInfo, getServerHost, getAvaIP, dispatcher);
		}

        public IServerInfo InitializeServices(Func<IServerInfo, IPlatformInfo> getPlatformInfo, Func<NativeServerHostConfig, IServerInfo, INativeServerHost> getServerHost, Func<Action, IServerInfo, ILocalIPCollection> getAvaIP, IThreadDispatcher dispatcher)
        {
			var container = new ServerInfo(dispatcher, this); // NOTE: For now, this is really a singleton internally

			// Hosting/general
			Server.ServerInfo.AddSingleton(getPlatformInfo);
			Server.ServerInfo.AddTransient(getServerHost);
			Server.ServerInfo.AddTransient(getAvaIP);
			Server.ServerInfo.AddSingleton<IHostingManager>(s => new HostingManager(s));

			// Features
			Server.ServerInfo.AddSingleton<ISwitcherList>(s => new SwitcherList(s));
			Server.ServerInfo.AddSingleton<IScriptButtonList>(s => new ScriptButtonList(s));
			Server.ServerInfo.AddTransient<ISwitcher>((s) => new Switcher(s));

			// Switcher
			Server.ServerInfo.AddTransient<ISwitcherFactory>(s => new SwitcherFactory(s));
			Server.ServerInfo.AddTransient<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new HotSwappableSwitcherInteractionBuffer(p, s));
			Server.ServerInfo.AddTransient<IPerSwitcherInteractionBuffer, SwitcherConfig>((p, s) => new PerSwitcherInteractionBuffer(p, s));
			Server.ServerInfo.AddTransient<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, IRawSwitcher>((p1, p2, s) => new PerSpecSwitcherInteractionBuffer(p1, p2, s));
			Server.ServerInfo.AddSingleton<ISwitcherInteractionBufferFactory>(s => new SwitcherInteractionBufferFactory());

			Server.ServerInfo.AddTransient<IVirtualSwitcher, VirtualSwitcherConfig>((p1, s) => new VirtualSwitcher(p1));
			Server.ServerInfo.AddSingleton<IATEMPlatformCompatibility>(s => new ATEMPlatformCompatibility(s));
			Server.ServerInfo.AddTransient<IATEMSwitcher, ATEMSwitcherConfig>((p1, s) => new ATEMSwitcher(p1, s));
			Server.ServerInfo.AddTransient<IATEMConnection, ATEMSwitcherConfig, IATEMSwitcher>((p1, p2, s) => new ATEMConnection(p1, p2, s));
			Server.ServerInfo.AddTransient<IATEMCallbackHandler, IATEMSwitcher>((p1, s) => new ATEMCallbackHandler(p1));

#pragma warning disable
			Server.ServerInfo.AddSingleton<INativeATEMSwitcherDiscovery>(s => new WindowsNativeATEMSwitcherDiscovery());
#pragma warning enable

			return container;
		}

		public ISwitcherList GetFeatures() => ServerInfo.Get<ISwitcherList>();
		public IHostingManager GetHostingManager() => ServerInfo.Get<IHostingManager>();
		public IScriptButtonList GetAutoButtons() => ServerInfo.Get<IScriptButtonList>();
		public IPlatformInfo GetPlatformInfo() => ServerInfo.Get<IPlatformInfo>();
	}
}
