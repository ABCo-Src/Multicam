using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.General;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using System.Data;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher
{
    public interface ISwitcherConnectionPresenter : IParameteredService<IFeature>
	{
		ISwitcherConnectionVM VM { get; }
		void OnError(string? error);
		void OnConnection(bool state);
		void OnSpecced(SwitcherSpecs specs);
	}

	public class SwitcherConnectionPresenter : ISwitcherConnectionPresenter
	{
		ISwitcherErrorPresenter _errorPresenter;
		IFeature _feature;

		bool _isConnected = false;

		readonly IMainThreadDispatcher _dispatcher;
		Timer? _transitionTimer;
		int _transitionState;

		public ISwitcherConnectionVM VM => _errorPresenter.VM;

		public static ISwitcherConnectionPresenter New(IFeature feature, IServiceSource servSource) => new SwitcherConnectionPresenter(feature, servSource);
		public SwitcherConnectionPresenter(IFeature feature, IServiceSource servSource)
		{
			_feature = feature;
			_errorPresenter = servSource.Get<ISwitcherErrorPresenter, IFeature, Action>(feature, ToggleConnection);
			_dispatcher = servSource.Get<IMainThreadDispatcher>();
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
				_feature.PerformAction(SwitcherActionID.DISCONNECT);
			else
			{
				_errorPresenter.SetErrorlessButtonVisible(false);
				StartNewTransitionTimer(GetConnectingText);

				_feature.PerformAction(SwitcherActionID.CONNECT);
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
			StopTransitionTimer();
			_errorPresenter.SetErrorlessButtonText("Disconnect");
			_errorPresenter.SetErrorlessStatus("Connected.");
			_errorPresenter.SetErrorlessButtonVisible(specs.CanChangeConnection);
		}

		void StartNewTransitionTimer(Func<string> updateText)
		{
			_transitionTimer = new Timer(o =>
			{
				_dispatcher.QueueOnMainFeatureThread(() =>
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
