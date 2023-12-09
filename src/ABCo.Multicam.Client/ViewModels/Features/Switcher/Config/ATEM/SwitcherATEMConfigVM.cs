using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.Presenters.Features.Switchers.Config;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher.Config.ATEM
{
    public interface ISwitcherATEMConfigVM : IClientService<ISwitcherATEMConfigPresenter>, ISwitcherSpecificConfigVM, INotifyPropertyChanged 
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

	public partial class SwitcherATEMConfigVM : ViewModelBase, ISwitcherATEMConfigVM
	{
		readonly ISwitcherATEMConfigPresenter _presenter;

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

		public SwitcherATEMConfigVM(ISwitcherATEMConfigPresenter presenter) => _presenter = presenter;

		public void OnIPChange() => _presenter.OnUIChange();
		public void OnSelectedTypeChange() => _presenter.OnUIChange();
	}
}
