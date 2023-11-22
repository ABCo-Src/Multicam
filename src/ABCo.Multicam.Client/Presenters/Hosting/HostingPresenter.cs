using ABCo.Multicam.Client.ViewModels.Hosting;
using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management;

namespace ABCo.Multicam.Client.Presenters.Hosting
{
	public interface IHostingPresenter : IClientDataNotificationTarget<IHostingManagerState, IHostingManager>
	{
		void OnHostingMenuToggle();
		void OnHostingModeChange();
		void OnCustomHostNameChange();
		void OnToggleConnection();
		IServerHostingVM VM { get; }
	}

	public class HostingPresenter : IHostingPresenter, IClientService<IServerComponent>
	{
		bool _menuOpen = false;
		readonly IMainUIPresenter _mainUI;
		readonly IHostingManagerState _state;
		readonly IDispatchedServerComponent<IHostingManager> _manager;

		public IServerHostingVM VM { get; }

		public HostingPresenter(IHostingManagerState state, IDispatchedServerComponent<IHostingManager> manager, IClientInfo info)
		{
			_state = state;
			_manager = manager;
			_mainUI = info.Get<IMainUIPresenter>();

			var hostnameConfigVM = info.Get<IHostnameConfigVM, IHostingPresenter>(this);
			var hostingExecutionVM = info.Get<IHostingExecutionVM, IHostingPresenter>(this);
			VM = info.Get<IServerHostingVM, IHostingPresenter, IHostnameConfigVM, IHostingExecutionVM>(this, hostnameConfigVM, hostingExecutionVM);
		}

		public void Init()
		{
			
		}

		public static string TrimLinkStart(string str) => str.StartsWith("http://") ? str[7..] : str;

		public void OnHostingMenuToggle()
		{
			if (_menuOpen)
				_mainUI.CloseMenu();
			else
			{
				_mainUI.OpenMenu(VM, "Hosting Options", () => _menuOpen = false);
				_menuOpen = true;
			}
		}

		public void OnHostingModeChange() => _manager.CallDispatched(m => m.SetMode(VM.HostnameVM.SelectedMode == "Automatic"));
		public void OnCustomHostNameChange()
		{
			var adaptedHostName = VM.HostnameVM.CustomHostName.StartsWith("http://") ? VM.HostnameVM.CustomHostName : $"http://{VM.HostnameVM.CustomHostName}";
			_manager.CallDispatched(m => m.SetCustomModeConfig(new string[] { adaptedHostName }));
		}

		public void OnToggleConnection() => _manager.CallDispatched(m => m.ToggleOnOff());

		public void OnServerStateChange(string? changedProp)
		{
			// Update the mode
			VM.HostnameVM.SelectedMode = _state.IsAutomatic ? "Automatic" : "Custom";

			// Update the custom host name
			var hostName = _state.CustomModeHostNames.Count > 0 ? _state.CustomModeHostNames[0] : "";
			VM.HostnameVM.CustomHostName = TrimLinkStart(hostName);

			// Update the execution status.
			VM.ShowConfigOptions = !_state.IsConnected;
			VM.ExecutionVM.StartStopButtonText = _state.IsConnected ? "Stop Hosting" : "Start Hosting";

			// Update the active config text
			if (_state.ActiveHostName == null)
			{
				VM.HostnameVM.AutomaticCaption = "Scanning hosts (ensure you're connected to a network)...";
				VM.ExecutionVM.CanStartStop = false;
			}
			else
			{
				VM.HostnameVM.AutomaticCaption = $"Using {_state.ActiveHostName}";
				VM.ExecutionVM.LinkText = TrimLinkStart(_state.ActiveHostName);
				VM.ExecutionVM.LinkHyperlink = _state.ActiveHostName;
				VM.ExecutionVM.CanStartStop = true;
			}
		}
	}
}
