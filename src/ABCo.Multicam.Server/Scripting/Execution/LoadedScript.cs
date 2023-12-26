using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Execution
{
    /// <summary>
    /// Represents an automation loaded into memory
    /// </summary>
    public class LoadedScript
    {
        Script _script;
        DynValue? _outerFunction;

        public LoadedScript(string code)
        {
            _script = new(CoreModules.Preset_HardSandbox);
            _outerFunction = _script.LoadString(code);
        }

        public void Load(string code)
        {
            _script.LoadString(code);
        }

        //public bool ContinueExecution()
        //{

        //}
    }
}
