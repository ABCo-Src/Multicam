using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Presenters.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherConnectionVM : IParameteredService<ISwitcherErrorPresenter>
	{
		string StatusText { get; set; }
		string StatusButtonText { get; set; }
		bool ShowConnectionButton { get; set; }
	}

	public partial class SwitcherConnectionVM : ViewModelBase, ISwitcherConnectionVM
	{
		ISwitcherErrorPresenter _errorPresenter;		

		public SwitcherConnectionVM(ISwitcherErrorPresenter errorPresenter) => _errorPresenter = errorPresenter;

		[ObservableProperty] string _statusText = "";
		[ObservableProperty] string _statusButtonText = "";
		[ObservableProperty] bool _showConnectionButton;

		public void ToggleConnection() => _errorPresenter.ButtonClick();
	}
}
