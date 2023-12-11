using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using ABCo.Multicam.Client.Presenters.Features.Switchers.Config.ATEM;
using ABCo.Multicam.Client.Presenters.Features.Switchers.Config.Dummy;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
	public interface ISwitcherConfigVM : INotifyPropertyChanged, IDisposable
	{
		string[] Items { get; }
		string SelectedItem { get; set; }
		ISwitcherSpecificConfigVM? CurrentConfig { get; set; }
		void UpdateSelectedItem();
    }

    public interface ISwitcherSpecificConfigVM : INotifyPropertyChanged, IDisposable
    {
    }

    public partial class SwitcherConfigVM : BoundViewModelBase<ISwitcher>, ISwitcherConfigVM
	{
        SwitcherType? _oldType;

		public string[] Items => new string[]
        {
			"Dummy",
			"ATEM"
        };

		[ObservableProperty] string _selectedItem = "Dummy";
		[ObservableProperty] ISwitcherSpecificConfigVM? _currentConfig;

		public SwitcherConfigVM(Dispatched<ISwitcher> feature, IClientInfo info) : base(feature, info) => OnServerStateChange(null);
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
					_ => "Dummy"
				};

				// Update the inner VM
				CurrentConfig?.Dispose();
				CurrentConfig = config.Type switch
				{
					SwitcherType.Dummy => new SwitcherDummyConfigVM(_serverComponent, _info),
					SwitcherType.ATEM => new SwitcherATEMConfigVM(_serverComponent, _info),
					_ => throw new Exception("Unsupported switcher type!")
				};
			}
		}

        public void UpdateSelectedItem()
        {
            _serverComponent.CallDispatched(f => f.ChangeConfig(SelectedItem switch
            {
                "Dummy" => new DummySwitcherConfig(4),
                "ATEM" => new ATEMSwitcherConfig(null),
                _ => throw new Exception("Unsupported selected mode given")
            }));
        }

		public override void Dispose()
		{
			CurrentConfig?.Dispose();
			base.Dispose();
		}
	}
}