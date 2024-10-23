using ABCo.Multicam.Server.Automation.Buttons;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management;
using ABCo.Multicam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Server.Scripting;
using ABCo.Multicam.Server.Scripting.Console;

namespace ABCo.Multicam.Client.Management
{
    public interface IServerConnection
    {
        Dispatched<T> CreateDispatcher<T>(T item);
        Dispatched<ISwitcherList> GetSwitcherList();
        Dispatched<IScriptConsole> GetScriptConsole();
        Dispatched<IScriptButtonList> GetScriptButtonList();
        Dispatched<IHostingManager> GetHostingManager();
        IPlatformInfo GetPlatformInfo();
    }

    public class ServerConnection : IServerConnection
    {
        readonly IServerInfo _info;
        public ServerConnection(IServerInfo info) => _info = info;

        public Dispatched<T> CreateDispatcher<T>(T native) => new(native, _info.Dispatcher);
        public Dispatched<ISwitcherList> GetSwitcherList() => CreateDispatcher(_info.Shared.SwitcherList);
        public Dispatched<IHostingManager> GetHostingManager() => CreateDispatcher(_info.Shared.HostingManager);
        public Dispatched<IScriptConsole> GetScriptConsole() => CreateDispatcher(_info.Shared.ScriptManager.Console);
        public Dispatched<IScriptButtonList> GetScriptButtonList() => CreateDispatcher(_info.Shared.ScriptManager.ButtonList);
        public IPlatformInfo GetPlatformInfo() => _info.PlatformInfo;
    }
}
