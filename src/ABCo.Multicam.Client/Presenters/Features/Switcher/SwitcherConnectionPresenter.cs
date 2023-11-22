using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public interface ISwitcherConnectionPresenter : IClientService<IDispatchedServerComponent<ISwitcherFeature>>
	{
		ISwitcherConnectionVM VM { get; }
		void OnError(string? error);
		void OnConnection(bool state);
		void OnSpecced(SwitcherSpecs specs);
	}

	public class SwitcherConnectionPresenter : ISwitcherConnectionPresenter
	{
		readonly ISwitcherErrorPresenter _errorPresenter;
		readonly IDispatchedServerComponent<ISwitcherFeature> _feature;

		bool _lastKnownConnection = false;
		bool _isWaitingForConnection;

		readonly IThreadDispatcher _dispatcher;
		Timer? _transitionTimer;
		int _transitionState;

		public ISwitcherConnectionVM VM => _errorPresenter.VM;

		public SwitcherConnectionPresenter(IDispatchedServerComponent<ISwitcherFeature> feature, IClientInfo clientInfo)
		{
			_feature = feature;
			_errorPresenter = clientInfo.Get<ISwitcherErrorPresenter, IDispatchedServerComponent<ISwitcherFeature>, Action>(feature, ToggleConnection);
			_dispatcher = clientInfo.Dispatcher;
		}

		public void OnError(string? error) => _errorPresenter.OnError(error);

		string GetConnectingText() => _transitionState switch
		{
			0 => "Connecting.",
			1 => "Connecting..",
			2 => "Connecting...",
			_ => throw new Exception("Unsupported UI transition state")
		};

		string GetReceivingDetailsText() => _transitionState switch
		{
			0 => "Retrieving Specs.",
			1 => "Retrieving Specs..",
			2 => "Retrieving Specs...",
			_ => throw new Exception("Unsupported UI transition state")
		};

		public void ToggleConnection()
		{
			StopTransitionTimer();

			// Handle connect/disconnect
			if (_lastKnownConnection) 
				_feature.CallDispatched(f => f.Disconnect());
			else
			{
				_errorPresenter.SetErrorlessButtonVisible(false);
				StartNewTransitionTimer(GetConnectingText);

				_feature.CallDispatched(f => f.Connect());
			}
		}

		public void OnConnection(bool state)
		{
			StopTransitionTimer();
			_lastKnownConnection = state;

			if (_lastKnownConnection)
			{
				_errorPresenter.SetErrorlessButtonVisible(false);
				StartNewTransitionTimer(GetReceivingDetailsText);
			}
			else
			{
				_errorPresenter.SetErrorlessButtonText("Connect");
				_errorPresenter.SetErrorlessStatus("Disconnected.");
				_errorPresenter.SetErrorlessButtonVisible(true);
			}
		}

		public void OnSpecced(SwitcherSpecs specs)
		{
			if (!_lastKnownConnection) return;

			StopTransitionTimer();
			_errorPresenter.SetErrorlessButtonText("Disconnect");
			_errorPresenter.SetErrorlessStatus("Connected.");
			_errorPresenter.SetErrorlessButtonVisible(specs.CanChangeConnection);
		}

		void StartNewTransitionTimer(Func<string> updateText)
		{
			_transitionTimer = new Timer(o =>
			{
				_dispatcher.Queue(() =>
				{
					// If we cancelled between the dispatcher call, don't process (otherwise we'll override the actual thing)
					if (_transitionTimer == null) return;

					_transitionState++;
					if (_transitionState == 3) _transitionState = 0;

					_errorPresenter.SetErrorlessStatus(updateText());
				});

			}, null, 0, 300);
		}

		void StopTransitionTimer()
		{
			if (_transitionTimer != null)
			{
				_transitionTimer.Dispose();
				_transitionTimer = null;
			}
		}
	}
}
