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
        //public SwitcherProxy this[string str] 
        //{ 
        //    get => 
        //}

        public string Id => "abc";


        public SwitchersGlobalProxy(ILoadedScript script)
        {

        }
    }
}
