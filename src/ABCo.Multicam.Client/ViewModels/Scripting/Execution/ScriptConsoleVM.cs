using ABCo.Multicam.Client.ViewModels.Paging;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Scripting.Console;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Scripting.Execution
{
    public interface IScriptConsoleVM : INotifyPropertyChanged
    {
        ConsoleMessage[] Messages { get; }
    }

    public partial class ScriptConsoleVM : BoundViewModelBase<IScriptConsole>, IPageVM, IScriptConsoleVM
    {
        [ObservableProperty] ConsoleMessage[] _messages = Array.Empty<ConsoleMessage>();

        public ScriptConsoleVM(Dispatched<IScriptConsole> serverComponent, IFrameClientInfo info) : base(serverComponent, info)
        {
            OnServerStateChange(null);
        }

        public AppPages Page => AppPages.ScriptConsole;

        protected override void OnServerStateChange(string? changedProp) => Messages = _serverComponent.Get(s => s.Messages);
    }
}
