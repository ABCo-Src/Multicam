using ABCo.Multicam.Client.ViewModels.Hosting;
using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management;
using ABCo.Multicam.Server.Hosting.Management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.Presenters.Hosting
{
	public interface IHostingPresenter : IClientDataNotificationTarget
	{
		void OnHostingMenuToggle();
		void OnHostingModeChange();
		void OnCustomHostNameChange();
		void OnToggleConnection();
		IServerHostingVM VM { get; }
	}

	public class HostingPresenter : IHostingPresenter, IClientService<IServerTarget>
	{
		bool _menuOpen = false;
		readonly IMainUIPresenter _mainUI;
		readonly IServerTarget _target;

		public IServerHostingVM VM { get; }

		public HostingPresenter(IServerTarget target, IClientInfo info)
		{
			_target = target;
			_mainUI = info.Get<IMainUIPresenter>();

			var hostnameConfigVM = info.Get<IHostnameConfigVM, IHostingPresenter>(this);
			var hostingExecutionVM = info.Get<IHostingExecutionVM, IHostingPresenter>(this);
			VM = info.Get<IServerHostingVM, IHostingPresenter, IHostnameConfigVM, IHostingExecutionVM>(this, hostnameConfigVM, hostingExecutionVM);
		}

		public void Init()
		{
			
		}

		public void OnDataChange(ServerData obj)
		{
			switch (obj)
			{
				case HostingConfigMode config:
					VM.HostnameVM.SelectedMode = config.IsAutomatic ? "Automatic" : "Custom";
					break;
				case HostingCustomModeConfig customConfig:
					var hostName = customConfig.HostNames.Count > 0 ? customConfig.HostNames[0] : "";
					VM.HostnameVM.CustomHostName = hostName.StartsWith("http://") ? hostName[7..] : hostName;
					break;
				case HostingExecutionStatus execStatus:
					VM.ExecutionVM.StartStopButtonText = execStatus.IsConnected ? "Stop Hosting" : "Start Hosting";
					break;
				case HostingActiveConfig activeConfig:

					if (activeConfig.HostName == null)
					{
						VM.HostnameVM.AutomaticCaption = "Scanning hosts (ensure you're connected to a network)...";
						VM.ExecutionVM.CanStartStop = false;
					}
					else
					{
						VM.HostnameVM.AutomaticCaption = $"Using {activeConfig.HostName}";
						VM.ExecutionVM.CanStartStop = true;
					}

					break;
			}
		}

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

		public void OnHostingModeChange() => _target.PerformAction(HostingManager.SET_MODE, new HostingConfigMode(VM.HostnameVM.SelectedMode == "Automatic"));
		public void OnCustomHostNameChange()
		{
			var adaptedHostName = VM.HostnameVM.SelectedMode.StartsWith("http://") ? VM.HostnameVM.SelectedMode : $"http://{VM.HostnameVM.SelectedMode}";
			_target.PerformAction(HostingManager.SET_CUSTOM_CONFIG, new HostingCustomModeConfig(new string[] { adaptedHostName }));
		}

		public void OnToggleConnection() => _target.PerformAction(HostingManager.TOGGLE_ONOFF);
	}
}
