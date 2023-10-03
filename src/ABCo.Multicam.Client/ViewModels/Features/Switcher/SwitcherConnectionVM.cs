using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
	public interface ISwitcherConnectionVM : IClientService<ISwitcherErrorPresenter>
	{
		string StatusText { get; set; }
		string StatusButtonText { get; set; }
		bool ShowConnectionButton { get; set; }
	}

	public partial class SwitcherConnectionVM : ViewModelBase, ISwitcherConnectionVM
	{
		readonly ISwitcherErrorPresenter _errorPresenter;		

		public SwitcherConnectionVM(ISwitcherErrorPresenter errorPresenter) => _errorPresenter = errorPresenter;

		[ObservableProperty] string _statusText = "";
		[ObservableProperty] string _statusButtonText = "";
		[ObservableProperty] bool _showConnectionButton;

		public void ToggleConnection() => _errorPresenter.ButtonClick();
	}
}
