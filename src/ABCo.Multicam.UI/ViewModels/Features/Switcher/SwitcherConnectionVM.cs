using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.General;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherConnectionVM : INeedsInitialization<ISwitcherRunningFeature>
	{
		void OnException(Exception? currentError);
		void OnConnection(bool state);
		void OnSpecced(SwitcherSpecs specs);

		string StatusText { get; }
		string StatusButtonText { get; }
		bool ShowConnectionButton { get; }
	}

	public partial class SwitcherConnectionVM : ViewModelBase, ISwitcherConnectionVM
	{
		ISwitcherRunningFeature _feature = null!;
		bool _isConnected = false;
		SwitcherSpecs? _specs;
		ConnectionState _state = ConnectionState.Stable;

		Exception? _currentError;

		IMainThreadDispatcher _dispatcher;
		Timer? _transitionTimer;
		int _transitionState;

		public SwitcherConnectionVM(IServiceSource servSource) => _dispatcher = servSource.Get<IMainThreadDispatcher>();
		public void FinishConstruction(ISwitcherRunningFeature baseFeature) => _feature = baseFeature;

		public string StatusText
		{
			get
			{
				if (_currentError != null) return $"Communication Error: {_currentError!.Message}";

				if (_isConnected)
					return _state switch
					{
						ConnectionState.Stable => "Connected.",
						ConnectionState.Transitioning => "Disconnecting...",
						ConnectionState.TransitioningWithSpecs => GetReceivingDetailsText(),
						_ => throw new Exception("Unrecognised state code")
					};
				else
					return _state switch
					{
						ConnectionState.Stable => "Disconnected",
						ConnectionState.Transitioning => GetConnectingText(),
						_ => throw new Exception("Unsupported state code")
					};
			}
		}

		public string StatusButtonText
		{
			get
			{
				if (_currentError != null) return "OK";
				return _isConnected ? "Disconnect" : "Connect";
			}
		}

		public bool ShowConnectionButton => _state == ConnectionState.Stable && _specs?.CanChangeConnection == true;

		string GetConnectingText() => _transitionState switch
		{
			0 => "Connecting.",
			1 => "Connecting..",
			2 => "Connecting...",
			_ => throw new Exception("Unsupported UI transition state")
		};

		string GetReceivingDetailsText() => _transitionState switch
		{
			0 => "Receiving Details.",
			1 => "Receiving Details..",
			2 => "Receiving Details...",
			_ => throw new Exception("Unsupported UI transition state")
		};

		public void OnException(Exception? currentError)
		{
			StopTransitionTimer();

			_currentError = currentError;
			OnPropertyChanged(nameof(StatusText));
		}

		public void OnConnection(bool state)
		{
			StopTransitionTimer();
			_isConnected = state;

			if (_isConnected)
			{
				_state = ConnectionState.TransitioningWithSpecs;
				StartNewTransitionTimer();
			}
			else
				_state = ConnectionState.Stable;
			
			OnPropertyChanged(nameof(StatusText));
		}

		public void OnSpecced(SwitcherSpecs specs)
		{
			_state = ConnectionState.Stable;
			_specs = specs;

			StopTransitionTimer();
			OnPropertyChanged(nameof(StatusText));
		}

		public void ToggleConnection()
		{
			StopTransitionTimer();

			// Handle errors
			if (_currentError != null)
				_currentError = null;

			// Handle connect/disconnect
			else
			{
				if (_isConnected)
					_feature.Disconnect();
				else
				{
					// Now we're connecting, so let's switch to that
					_state = ConnectionState.Transitioning;
					StartNewTransitionTimer();

					_feature.Connect();
				}
			}

			OnPropertyChanged(nameof(StatusText));
		}

		private void StartNewTransitionTimer()
		{
			_transitionTimer = new Timer(o =>
			{
				_dispatcher.QueueOnMainFeatureThread(() =>
				{
					_transitionState++;
					if (_transitionState == 3) _transitionState = 0;

					OnPropertyChanged(nameof(StatusText));
				});

			}, null, 0, 300);
		}

		private void StopTransitionTimer()
		{
			if (_transitionTimer != null)
			{
				_transitionTimer.Dispose();
				_transitionTimer = null;
			}
		}

		enum ConnectionState
		{
			Stable,
			Transitioning,
			TransitioningWithSpecs
		}
	}
}
