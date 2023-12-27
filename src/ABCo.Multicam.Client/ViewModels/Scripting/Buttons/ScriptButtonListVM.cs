using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.General;
using ABCo.Multicam.Client.ViewModels.Paging;
using ABCo.Multicam.Server.Automation.Buttons;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Scripting.Buttons
{
    public interface IScriptButtonListVM : IServerListVM<IScriptButtonListItemVM>, IPageVM, INotifyPropertyChanged
    {
    }

    public partial class ScriptButtonListVM : ServerListVM<IScriptButtonList, IScriptButton, IScriptButtonListItemVM>, IScriptButtonListVM
    {
        public AppPages Page => AppPages.ScriptButtons;

        public ScriptButtonListVM(Dispatched<IScriptButtonList> collection, IFrameClientInfo client) : base(AppPages.ScriptButtons, collection, client) { }

        protected override void OnServerStateChange(string? changedProp) =>
            ProcessServerListChange(s => new ScriptButtonListItemVM(_serverComponent, _info.Server.CreateDispatcher(s), _info));

        public void Create() => _serverComponent.CallDispatched(c => c.CreateAutomation());
    }
}
