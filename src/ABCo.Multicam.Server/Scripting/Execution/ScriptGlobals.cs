using ABCo.Multicam.Server.Scripting.Console;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Execution
{
    public interface IScriptGlobals
    {

    }

    public class ScriptGlobals : IScriptGlobals
    {
        

        public ScriptGlobals(ILoadedScript script)
        {
            
        }

        //public Script CreateScript()
        //{
        //    var script = new Script();

        //    // Add the switchers proxy.
        //    script.Globals["Switchers"] = 
        //}
    }
}
