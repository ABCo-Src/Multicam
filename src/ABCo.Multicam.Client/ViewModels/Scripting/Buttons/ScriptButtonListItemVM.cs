using ABCo.Multicam.Client.ViewModels.General;
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
    public interface IScriptButtonListItemVM : INamedMovableListItemVM<IScriptButtonVM>, INotifyPropertyChanged, IDisposable
    {
    }
    
    public class ScriptButtonListItemVM : NamedMovableBoundListItemVM<IScriptButtonList, IScriptButton, IScriptButtonVM>, IScriptButtonListItemVM
    {
        public ScriptButtonListItemVM(Dispatched<IScriptButtonList> list, Dispatched<IScriptButton> feature, IFrameClientInfo info) 
            : base(list, feature, new ScriptButtonVM(), info)
        {
        }
    }
}
