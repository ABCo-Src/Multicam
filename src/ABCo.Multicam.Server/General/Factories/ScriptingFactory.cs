using ABCo.Multicam.Server.Automation.Buttons;
using ABCo.Multicam.Server.Scripting.Console;
using ABCo.Multicam.Server.Scripting.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General.Factories
{
    public interface IScriptingFactory
    {
        IScriptButton CreateButton();
        IEditableScript CreateEditableScript(IScriptID id);
    }

    public class ScriptingFactory : IScriptingFactory
    {
        readonly IServerInfo _info;
        public ScriptingFactory(IServerInfo info) => _info = info;
        public IScriptButton CreateButton() => new ScriptButton(_info);
        public IEditableScript CreateEditableScript(IScriptID id) => new EditableScript(id, _info);
    }
}
