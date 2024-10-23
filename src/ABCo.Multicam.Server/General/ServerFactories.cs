using ABCo.Multicam.Server.General.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General
{
    public interface IServerFactories
    {
        IScriptingFactory Scripting { get; }
        ISwitcherFactory Switcher { get; }
        IHostingFactory Hosting { get; }
    }

    public class ServerFactories : IServerFactories
    {
        public IScriptingFactory Scripting { get; }
        public ISwitcherFactory Switcher { get; }
        public IHostingFactory Hosting { get; }

        public ServerFactories(IHostingFactory hosting, IServerInfo info)
        {
            Scripting = new ScriptingFactory(info);
            Switcher = new SwitcherFactory(info);
            Hosting = hosting;
        }
    }
}
