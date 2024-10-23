using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using ABCo.Multicam.Client.Presenters.Features.Switchers.Config.ATEM;
using System.ComponentModel;
using ABCo.Multicam.Client.Structures;
using ABCo.Multicam.Client.Presenters.Features.Switchers.Config.Virtual;
using ABCo.Multicam.Client.ViewModels.Features.Switchers.Config.OBS;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
    public interface ISwitcherConfigVM : INotifyPropertyChanged, IDisposable
    {
        string[] Items { get; }
        string SwitcherTypeTitle { get; }
        string SelectedItem { get; set; }
        ISwitcherSpecificConfigVM? CurrentConfig { get; set; }
        void UpdateSelectedItem();
        void OpenEditMenu(CursorPosition pos);
    }

    public interface ISwitcherSpecificConfigVM : INotifyPropertyChanged, IDisposable
    {
    }

    public partial class SwitcherConfigVM : BoundViewModelBase<ISwitcher>, ISwitcherConfigVM, IPopOutContentVM
    {
        SwitcherType? _oldType;

        public string[] Items => new string[]
        {
            "Virtual",
            "ATEM",
            "OBS"
        };

        public string SwitcherTypeTitle => SelectedItem + " Switcher";

        public string Title => "Connection Settings";

        [ObservableProperty] string _selectedItem = "Virtual";
        [ObservableProperty] ISwitcherSpecificConfigVM? _currentConfig;

        public SwitcherConfigVM(Dispatched<ISwitcher> feature, IFrameClientInfo info) : base(feature, info) => OnServerStateChange(null);
        protected override void OnServerStateChange(string? changedProp)
        {
            var config = _serverComponent.Get(m => m.Config);

            // If the config's type changed, create a new VM for the type 
            if (config.Type != _oldType)
            {
                _oldType = config.Type;

                // Set the selected item
                SelectedItem = config.Type switch
                {
                    SwitcherType.ATEM => "ATEM",
                    SwitcherType.OBS => "OBS",
                    _ => "Virtual"
                };

                // Update the inner VM
                CurrentConfig?.Dispose();
                CurrentConfig = config.Type switch
                {
                    SwitcherType.Virtual => new SwitcherVirtualConfigVM(_serverComponent, _info),
                    SwitcherType.ATEM => new SwitcherATEMConfigVM(_serverComponent, _info),
                    SwitcherType.OBS => new SwitcherOBSConfigVM(_serverComponent, _info),
                    _ => throw new Exception("Unsupported switcher type!")
                };
            }
        }

        public void UpdateSelectedItem()
        {
            _serverComponent.CallDispatched(f => f.ChangeConfig(SelectedItem switch
            {
                "Virtual" => new VirtualSwitcherConfig(4),
                "ATEM" => new ATEMSwitcherConfig(null),
                "OBS" => new OBSSwitcherConfig("", 0, ""),
                _ => throw new Exception("Unsupported selected mode given")
            }));
        }

        public override void Dispose()
        {
            CurrentConfig?.Dispose();
            base.Dispose();
        }

        public void OpenEditMenu(CursorPosition pos) => _info.Shared.PopOut.Open(this, pos);
    }
}