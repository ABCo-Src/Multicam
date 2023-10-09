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
		void ToggleConnection();
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
			VM = info.Get<IServerHostingVM, IHostingPresenter>(this);
		}

		public void Init()
		{
			
		}

		public void OnDataChange(ServerData obj)
		{
			switch (obj)
			{
				case HostingConfigMode config:
					VM.SelectedMode = config.IsAutomatic ? "Automatic" : "Custom";
					break;
				case HostingCustomModeConfig customConfig:
					var hostName = customConfig.HostNames.Count > 0 ? customConfig.HostNames[0] : "";
					VM.CustomHostName = hostName.StartsWith("http://") ? hostName[7..] : hostName;
					break;
				case HostingExecutionStatus execStatus:
					VM.StartStopButtonText = execStatus.IsConnected ? "Stop Hosting" : "Start Hosting";
					break;
				case HostingActiveConfig activeConfig:

					if (activeConfig.HostName == null)
					{
						VM.AutomaticCaption = "Scanning hosts (ensure you're connected to a network)...";
						VM.CanStartStop = false;
					}
					else
					{
						VM.AutomaticCaption = $"Using {activeConfig.HostName}";
						VM.CanStartStop = true;
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

		public void OnHostingModeChange() => _target.PerformAction(HostingManager.SET_MODE, new HostingConfigMode(VM.SelectedMode == "Automatic"));
		public void OnCustomHostNameChange()
		{
			var adaptedHostName = VM.CustomHostName.StartsWith("http://") ? VM.CustomHostName : $"http://{VM.CustomHostName}";
			_target.PerformAction(HostingManager.SET_CUSTOM_CONFIG, new HostingCustomModeConfig(new string[] { adaptedHostName }));
		}

		public void ToggleConnection() => _target.PerformAction(HostingManager.TOGGLE_ONOFF);
	}
}
