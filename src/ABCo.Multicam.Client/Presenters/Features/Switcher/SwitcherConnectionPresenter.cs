using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;
using System.Data;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public class SwitcherConnectionPresenter
	{
		readonly Dispatched<ISwitcherFeature> _feature;
		readonly IThreadDispatcher _dispatcher;
		int _transitionState;

		public ISwitcherConnectionVM VM { get; }

		public SwitcherConnectionPresenter(Dispatched<ISwitcherFeature> feature, IClientInfo clientInfo)
		{
			_feature = feature;
			VM = clientInfo.Get<ISwitcherConnectionVM, SwitcherConnectionPresenter>(this);
			_dispatcher = clientInfo.Dispatcher;
		}

		public void Refresh()
		{
			// With error
			var error = _feature.Get(f => f.ErrorMessage);
			if (error != null)
			{
				VM.StatusButtonText = "OK";
				VM.ShowConnectionButton = true;
				VM.StatusText = $"Communication Error: {error}";
			}

			// No error
			else
			{
				switch (_feature.Get(f => f.ConnectionStatus))
				{
					case SwitcherConnectionStatus.NotConnected:
						VM.StatusButtonText = "Connect";
						VM.ShowConnectionButton = true;
						VM.StatusText = "Disconnected.";
						break;
					case SwitcherConnectionStatus.Connecting:
						MoveDotsAndRefreshAfterInterval();
						VM.StatusText = GetConnectingText();
						VM.ShowConnectionButton = false;
						break;
					case SwitcherConnectionStatus.Connected:
						VM.StatusButtonText = "Disconnect";
						VM.ShowConnectionButton = _feature.Get(f => f.SpecsInfo).Specs.CanChangeConnection;
						VM.StatusText = "Connected.";
						break;
				}
			}
		}

		string GetConnectingText() => _transitionState switch
		{
			0 => "Connecting.",
			1 => "Connecting..",
			2 => "Connecting...",
			_ => throw new Exception("Unsupported UI transition state")
		};

		public void ToggleConnection()
		{
			// Handle connect/disconnect
			if (_feature.Get(f => f.ConnectionStatus) == SwitcherConnectionStatus.Connected) 
				_feature.CallDispatched(f => f.Disconnect());
			else
			{
				VM.ShowConnectionButton = false;
				_feature.CallDispatched(f => f.Connect());
			}
		}

		async void MoveDotsAndRefreshAfterInterval()
		{
			await Task.Delay(300);

			_transitionState++;
			if (_transitionState == 3) _transitionState = 0;

			Refresh();
		}
	}
}
