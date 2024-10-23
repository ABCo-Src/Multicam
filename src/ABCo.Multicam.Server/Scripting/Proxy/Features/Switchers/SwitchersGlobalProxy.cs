using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Scripting.Execution;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Proxy.Features.Switchers
{
    [MoonSharpUserData]
    public class SwitchersGlobalProxy
    {
        readonly ISwitcherList _switcherList;

        public SwitcherProxy? this[string str]
        {
            get
            {
                // Find the switcher
                var switcher = _switcherList.Items.FirstOrDefault(s => s.Name == str);
                if (switcher == null) 
                    throw new Exception($"Can't find switcher in the list with the name '{str}'. Make the relevant switcher have this name, or update the script.");

                return new SwitcherProxy(switcher);
            }
        }

        public SwitchersGlobalProxy(ILoadedScript script, IServerInfo info) => _switcherList = info.Shared.SwitcherList;

        public override string ToString() => "Switchers";
    }
}