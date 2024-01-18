using ABCo.Multicam.Server.Automation.Buttons;
using ABCo.Multicam.Server.General.Factories;
using ABCo.Multicam.Server.Scripting.Console;
using ABCo.Multicam.Server.Scripting.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting
{
    public interface IScriptManager
    {
        IScriptExecutionManager Execution { get; }
        IScriptConsole Console { get; }
        IScriptButtonList ButtonList { get; }

		IEditableScript NewScript(IScriptID id);
        void UnloadScript(IEditableScript script);
        void HandleScriptError(IScriptID id, Exception ex);
	}

    public class ScriptManager : IScriptManager
    {
		readonly List<IEditableScript> _registeredScripts = new();
        readonly IScriptingFactory _factory;

		public IScriptExecutionManager Execution { get; }
		public IScriptConsole Console { get; }
		public IScriptButtonList ButtonList { get; }

		public ScriptManager(IServerInfo info)
        {
			_factory = info.Factories.Scripting;
            Execution = new ScriptExecutionManager(info);
            ButtonList = new ScriptButtonList(info);
            Console = new ScriptConsole();
        }

        public IEditableScript NewScript(IScriptID id)
        {
            var newScript = _factory.CreateEditableScript(id);
            _registeredScripts.Add(newScript);
            return newScript;
        }

		public void UnloadScript(IEditableScript script) => _registeredScripts.Remove(script);

		public void HandleScriptError(IScriptID id, Exception ex) => Console.WriteLine(ex.Message, id, ConsoleMessageType.Error);
	}
}
