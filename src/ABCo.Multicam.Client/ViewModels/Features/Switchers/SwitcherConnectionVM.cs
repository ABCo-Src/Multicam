using ABCo.Multicam.Client.Structures;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
	public interface ISwitcherConnectionVM : INotifyPropertyChanged, IDisposable
    {
		string StatusText { get; set; }
		string StatusButtonText { get; set; }
		bool ShowConnectionButton { get; set; }
		bool ShowConnectionInfo { get; set; }
		void ToggleConnection();
	}

    public partial class SwitcherConnectionVM : BoundViewModelBase<ISwitcher>, ISwitcherConnectionVM
	{
        int _transitionState;

		[ObservableProperty] string _statusText = "";
		[ObservableProperty] string _statusButtonText = "";
		[ObservableProperty] bool _showConnectionInfo;
		[ObservableProperty] bool _showConnectionButton;

		public SwitcherConnectionVM(Dispatched<ISwitcher> feature, IClientInfo clientInfo) : base(feature, clientInfo) => OnServerStateChange(null);
		protected override async void OnServerStateChange(string? changedProp)
		{
			// With error
			var error = _serverComponent.Get(f => f.ErrorMessage);
			if (error != null)
			{
				StatusButtonText = "OK";
				StatusText = $"Communication Error: {error}";
			}

			// No error
			else
			{
				switch (_serverComponent.Get(f => f.ConnectionStatus))
				{
					case SwitcherConnectionStatus.NotConnected:
						StatusButtonText = "Connect";
						ShowConnectionButton = true;
						ShowConnectionInfo = true;
						StatusText = "Disconnected.";
						break;
					case SwitcherConnectionStatus.Connecting:
						ShowConnectionButton = false;
						ShowConnectionInfo = true;

						// Repeatedly update the text until we've stopped connecting
						while (_serverComponent.Get(f => f.ConnectionStatus) == SwitcherConnectionStatus.Connecting)
						{
							MoveDotsForwardOne();
							StatusText = GetConnectingText();
							await Task.Delay(300);
						}

						break;
					case SwitcherConnectionStatus.Connected:
						StatusButtonText = "Disconnect";
						ShowConnectionButton = true;
						ShowConnectionInfo = true; //_serverComponent.Get(f => f.SpecsInfo).Specs.CanChangeConnection;
						StatusText = "Connected.";
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
            // Handle an error
            if (_serverComponent.Get(f => f.ErrorMessage) != null)
            {
				_serverComponent.CallDispatched(f => f.AcknowledgeError());
                return;
            }

            // Handle connect/disconnect
            if (_serverComponent.Get(f => f.ConnectionStatus) == SwitcherConnectionStatus.Connected)
				_serverComponent.CallDispatched(f => f.Disconnect());
            else
				_serverComponent.CallDispatched(f => f.Connect());
        }

        void MoveDotsForwardOne()
        {
            _transitionState++;
            if (_transitionState == 3) _transitionState = 0;
        }
	}
}
