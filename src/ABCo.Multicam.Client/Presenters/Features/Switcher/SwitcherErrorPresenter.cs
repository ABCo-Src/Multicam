using ABCo.Multicam.Server;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Features.Switchers;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public interface ISwitcherErrorPresenter : IClientService<IServerTarget, Action>
	{
		ISwitcherConnectionVM VM { get; }
		void OnError(string? error);
		void SetErrorlessStatus(string status);
		void SetErrorlessButtonText(string text);
		void SetErrorlessButtonVisible(bool visible);
		void ButtonClick();
	}
	
	public class SwitcherErrorPresenter : ISwitcherErrorPresenter
	{
		readonly IServerTarget _feature;
		readonly Action _noErrorClick;
		string? _currentError = null;

		public ISwitcherConnectionVM VM { get; }

		public SwitcherErrorPresenter(IServerTarget feature, Action noErrorClick, IClientInfo servSource)
		{
			_feature = feature;
			_noErrorClick = noErrorClick;

			VM = servSource.Get<ISwitcherConnectionVM, ISwitcherErrorPresenter>(this);
		}

		public void OnError(string? error)
		{
			_currentError = error;

			if (_currentError == null)
			{
				VM.StatusButtonText = _errorlessButtonText;
				VM.ShowConnectionButton = _errorlessButtonVisible;
				VM.StatusText = _errorlessStatusText;
			}
			else
			{
				VM.StatusButtonText = "OK";
				VM.ShowConnectionButton = true;
				VM.StatusText = $"Communication Error: {_currentError}";
			}
		}

		string _errorlessButtonText = "";
		public void SetErrorlessButtonText(string text)
		{
			_errorlessButtonText = text;
			if (_currentError == null) VM.StatusButtonText = _errorlessButtonText;
		}

		bool _errorlessButtonVisible = true;
		public void SetErrorlessButtonVisible(bool visible)
		{
			_errorlessButtonVisible = visible;
			if (_currentError == null) VM.ShowConnectionButton = _errorlessButtonVisible;
		}

		string _errorlessStatusText = "";
		public void SetErrorlessStatus(string status)
		{
			_errorlessStatusText = status;
			if (_currentError == null) VM.StatusText = _errorlessStatusText;
		}

		public void ButtonClick()
		{
			if (_currentError != null) 
				_feature.PerformAction(SwitcherLiveFeature.ACKNOWLEDGE_ERROR);
			else 
				_noErrorClick();
		}
	}
}