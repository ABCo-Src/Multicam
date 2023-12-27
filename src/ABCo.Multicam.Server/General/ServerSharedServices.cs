using ABCo.Multicam.Server.Automation.Buttons;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers.Core.ATEM.Native;
using ABCo.Multicam.Server.Hosting.Management;
using ABCo.Multicam.Server.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General
{
	public interface IServerSharedServices
	{
		ISwitcherList SwitcherList { get; }
		IScriptButtonList ScriptButtonList { get; }
		IScriptManager ScriptManager { get; }
		IHostingManager HostingManager { get; }
		INativeATEMSwitcherDiscovery NativeATEMDiscovery { get; }
	}

	public class ServerSharedServices : IServerSharedServices
	{
		readonly IServerInfo _info;

		ISwitcherList? _switcherList;
		IScriptButtonList? _scriptBtnList;
		IScriptManager? _scriptManager;
		IHostingManager? _hostingManager;
		INativeATEMSwitcherDiscovery? _atemDiscovery;

		public ISwitcherList SwitcherList => _switcherList ??= new SwitcherList(_info);
		public IScriptButtonList ScriptButtonList => _scriptBtnList ??= new ScriptButtonList(_info);
		public IScriptManager ScriptManager => _scriptManager ??= new ScriptManager(_info);
		public IHostingManager HostingManager => _hostingManager ??= new HostingManager(_info);
#pragma warning disable
		public INativeATEMSwitcherDiscovery NativeATEMDiscovery => _atemDiscovery ??= new WindowsNativeATEMSwitcherDiscovery();
#pragma warning enable

		public ServerSharedServices(IServerInfo info) => _info = info;
	}
}
