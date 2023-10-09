using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Hosting;
using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Hosting
{
	public interface IServerHostingVM : INotifyPropertyChanged, IClientService<IHostingPresenter>, ISideMenuEmbeddableVM
	{
		string[] ModeValues { get; }
		string SelectedMode { get; set; }
		string AutomaticCaption { get; set; }
		string CustomHostName { get; set; }
		string StartStopButtonText { get; set; }
		bool ShowAutomaticCaption { get; }
		bool ShowCustomHostSelection { get; }
		bool CanStartStop { get; set; }
		void ToggleHostingMenu();
		void UpdateHostingMode();
		void UpdateCustomHostName();
		void ToggleConnection();
	}

	public interface IConnectedHostingVM
	{

	}

	public partial class ServerHostingVM : ViewModelBase, IServerHostingVM
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
		[ObservableProperty] string _startStopButtonText = "";
		[ObservableProperty] bool _canStartStop = false;

		public bool ShowCustomHostSelection => SelectedMode == "Custom";
		public bool ShowAutomaticCaption => SelectedMode == "Automatic";

		public ServerHostingVM(IHostingPresenter presenter) => _presenter = presenter;
		public void ToggleHostingMenu() => _presenter.OnHostingMenuToggle();
		public void UpdateHostingMode() => _presenter.OnHostingModeChange();
		public void UpdateCustomHostName() => _presenter.OnCustomHostNameChange();
		public void ToggleConnection() => _presenter.ToggleConnection();
	}
}