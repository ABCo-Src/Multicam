﻿using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public interface ISwitcherConnectionPresenter : IClientService<IServerTarget>
	{
		ISwitcherConnectionVM VM { get; }
		void OnError(string? error);
		void OnConnection(bool state);
		void OnSpecced(SwitcherSpecs specs);
	}

	public class SwitcherConnectionPresenter : ISwitcherConnectionPresenter
	{
		readonly ISwitcherErrorPresenter _errorPresenter;
		readonly IServerTarget _feature;

		bool _isConnected = false;

		readonly IThreadDispatcher _dispatcher;
		Timer? _transitionTimer;
		int _transitionState;

		public ISwitcherConnectionVM VM => _errorPresenter.VM;

		public SwitcherConnectionPresenter(IServerTarget feature, IClientInfo clientInfo)
		{
			_feature = feature;
			_errorPresenter = clientInfo.Get<ISwitcherErrorPresenter, IServerTarget, Action>(feature, ToggleConnection);
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
			if (_isConnected) 
				_feature.PerformAction(SwitcherLiveFeature.DISCONNECT);
			else
			{
				_errorPresenter.SetErrorlessButtonVisible(false);
				StartNewTransitionTimer(GetConnectingText);

				_feature.PerformAction(SwitcherLiveFeature.CONNECT);
			}
		}

		public void OnConnection(bool state)
		{
			StopTransitionTimer();
			_isConnected = state;

			if (_isConnected)
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
			if (!_isConnected) return;

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
