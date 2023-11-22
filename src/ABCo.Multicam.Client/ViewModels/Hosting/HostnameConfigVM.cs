using ABCo.Multicam.Client.Presenters.Hosting;
using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Hosting
{
	public interface IHostnameConfigVM : INotifyPropertyChanged, IClientService<IHostingPresenter>
	{
		string[] ModeValues { get; }
		string SelectedMode { get; set; }
		string AutomaticCaption { get; set; }
		string CustomHostName { get; set; }
		bool ShowAutomaticCaption { get; }
		bool ShowCustomHostSelection { get; }
		void UpdateHostingMode();
		void UpdateCustomHostName();
	}

	public partial class HostnameConfigVM : ViewModelBase, IHostnameConfigVM
	{
		readonly IHostingPresenter _presenter;

		public string[] ModeValues => new string[]
		{
			"Automatic",
			"Custom"
		};

		[ObservableProperty][NotifyPropertyChangedFor(nameof(ShowCustomHostSelection), nameof(ShowAutomaticCaption))] string _selectedMode = "Automatic";
		[ObservableProperty] string _automaticCaption = "";
		[ObservableProperty] string _customHostName = "";

		public HostnameConfigVM(IHostingPresenter presenter) => _presenter = presenter;

		public bool ShowCustomHostSelection => SelectedMode == "Custom";
		public bool ShowAutomaticCaption => SelectedMode == "Automatic";

		public void UpdateHostingMode() => _presenter.OnHostingModeChange();
		public void UpdateCustomHostName() => _presenter.OnCustomHostNameChange();
	}
}
