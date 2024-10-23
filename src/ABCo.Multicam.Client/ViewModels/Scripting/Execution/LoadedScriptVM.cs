using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Scripting.Execution;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Scripting.Execution
{
    public interface ILoadedScriptVM : INotifyPropertyChanged
    {
        bool IsRunning { get; }
        void ToggleExecution();
    }

    public partial class LoadedScriptVM : BoundViewModelBase<ILoadedScript>, ILoadedScriptVM
    {
        [ObservableProperty] bool _isRunning;

        public LoadedScriptVM(Dispatched<ILoadedScript> serverComponent, IFrameClientInfo info) : base(serverComponent, info)
        {
        }

        protected override void OnServerStateChange(string? changedProp)
        {
            IsRunning = _serverComponent.Get(s => s.IsRunning);
        }

        public void ToggleExecution()
        {
            if (IsRunning)
                _serverComponent.CallDispatched(s => s.Stop());
            else
                _serverComponent.CallDispatched(s => s.Start());
        }
    }
}
