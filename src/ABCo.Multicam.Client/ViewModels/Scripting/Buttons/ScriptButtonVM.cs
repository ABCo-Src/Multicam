using ABCo.Multicam.Client.ViewModels.Scripting.Editor;
using ABCo.Multicam.Server.Automation.Buttons;
using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Scripting.Buttons
{
    public interface IScriptButtonVM
    {
        IEditableScriptVM Script { get; }
    }

    public class ScriptButtonVM : IScriptButtonVM
    {
        readonly Dispatched<IScriptButton> _button;

        public IEditableScriptVM Script { get; }

        public ScriptButtonVM(Dispatched<IScriptButton> button, IFrameClientInfo info)
        {
            var innerScript = button.Get(b => b.Script);
            Script = new EditableScriptVM(info.Server.CreateDispatcher(innerScript), info);
        }
    }
}
