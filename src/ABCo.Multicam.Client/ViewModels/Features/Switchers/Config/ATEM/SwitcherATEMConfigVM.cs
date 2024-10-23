using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers.Config.ATEM
{
    public interface ISwitcherATEMConfigVM : ISwitcherSpecificConfigVM, INotifyPropertyChanged
    {
        string[] ConnectionTypes { get; }
        bool IsIPAddressEditable { get; }
        string SelectedConnectionType { get; set; }
        SwitcherPlatformCompatibilityValue CompatibilityMessage { get; set; }
        bool ShowOneProgramMessage { get; }
        string IpAddress { get; set; }
        void OnIPChange();
        void OnSelectedTypeChange();
    }

    public partial class SwitcherATEMConfigVM : BoundViewModelBase<ISwitcher>, ISwitcherATEMConfigVM
    {
        public string[] ConnectionTypes => new string[]
        {
            "USB",
            "IP"
        };

        public bool ShowOneProgramMessage => SelectedConnectionType == "USB";
        public bool IsIPAddressEditable => SelectedConnectionType == "IP";

        [ObservableProperty] string _ipAddress = "";
        [ObservableProperty] string _selectedConnectionType = "USB";
        [ObservableProperty] SwitcherPlatformCompatibilityValue _compatibilityMessage = SwitcherPlatformCompatibilityValue.Supported;

        public SwitcherATEMConfigVM(Dispatched<ISwitcher> feature, IFrameClientInfo info) : base(feature, info) => OnServerStateChange(null);

        public void OnUIChange()
        {
            var newATEMConfig = new ATEMSwitcherConfig(SelectedConnectionType == "USB" ? null : IpAddress);
            _serverComponent.CallDispatched(f => f.ChangeConfig(newATEMConfig));
        }

        protected override void OnServerStateChange(string? changedProp)
        {
            var config = _serverComponent.Get(m => m.Config);
            if (config is ATEMSwitcherConfig atemConfig)
            {
                // Update IP address info
                SelectedConnectionType = atemConfig.IP == null ? "USB" : "IP";
                if (atemConfig.IP != null)
                    IpAddress = atemConfig.IP;

                CompatibilityMessage = _serverComponent.Get(m => m.PlatformCompatibility);
            }
        }

        public void OnIPChange() => OnUIChange();
        public void OnSelectedTypeChange() => OnUIChange();
    }
}
