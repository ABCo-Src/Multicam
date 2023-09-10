using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher
{
	public interface ISwitcherErrorPresenter : IParameteredService<IFeature, Action>
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
		IFeature _feature;
		Action _noErrorClick;
		string? _currentError = null;

		public ISwitcherConnectionVM VM { get; }

		public SwitcherErrorPresenter(IFeature feature, Action noErrorClick, IServiceSource servSource)
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
				_feature.InteractionHandler.PerformAction(SwitcherActionID.ACKNOWLEDGE_ERROR);
			else 
				_noErrorClick();
		}
	}
}