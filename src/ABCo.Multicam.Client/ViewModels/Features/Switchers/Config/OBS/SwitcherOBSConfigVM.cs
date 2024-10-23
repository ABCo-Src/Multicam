using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Features.Switchers.Config.OBS
{
    public interface ISwitcherOBSConfigVM : ISwitcherSpecificConfigVM, INotifyPropertyChanged
    {
        string Ip { get; set; }
        string Port { get; set; }
        string Password { get; set; }
        string? ErrorMessage { get; set; }
        void OnIPChange();
        void OnPortChange();
        void OnPasswordChange();
    }

    public partial class SwitcherOBSConfigVM : BoundViewModelBase<ISwitcher>, ISwitcherOBSConfigVM
    {
        [ObservableProperty] string _ip = "";
        [ObservableProperty] string _port = "";
        [ObservableProperty] string _password = "";
        [ObservableProperty] string? _errorMessage = null;

        public SwitcherOBSConfigVM(Dispatched<ISwitcher> serverComponent, IFrameClientInfo info) : base(serverComponent, info) => OnServerStateChange(null);

        protected override void OnServerStateChange(string? changedProp)
        {
            var config = _serverComponent.Get(m => m.Config);
            if (config is OBSSwitcherConfig obsConfig)
            {
                // Update IP address info
                Ip = obsConfig.IP ?? "";
                Port = obsConfig.Port.ToString();
                Password = obsConfig.Password;
            }
        }

        void OnChange()
        {
            // Validate the port
            if (int.TryParse(Port, out int port))
            {
                ErrorMessage = "Server Port must be a valid number.";
                return;
            }

            if (port < 0)
            {
                ErrorMessage = "Port cannot be less than 0.";
                return;
            }

            // Send this data
            var newOBSConfig = new OBSSwitcherConfig(Ip, port, Password);
            _serverComponent.CallDispatched(f => f.ChangeConfig(newOBSConfig));
        }

        public void OnIPChange() => OnChange();
        public void OnPortChange() => OnChange();
        public void OnPasswordChange() => OnChange();
    }
}
