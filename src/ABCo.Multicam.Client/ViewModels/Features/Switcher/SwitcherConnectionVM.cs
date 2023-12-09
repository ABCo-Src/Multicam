using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using ABCo.Multicam.Client.Presenters.Features.Switchers;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
    public interface ISwitcherConnectionVM : IClientService<SwitcherConnectionPresenter>
	{
		string StatusText { get; set; }
		string StatusButtonText { get; set; }
		bool ShowConnectionButton { get; set; }
	}

	public partial class SwitcherConnectionVM : ViewModelBase, ISwitcherConnectionVM
	{
		readonly SwitcherConnectionPresenter _connectionPresenter;

		public SwitcherConnectionVM(SwitcherConnectionPresenter connectionPresenter) => _connectionPresenter = connectionPresenter;

		[ObservableProperty] string _statusText = "";
		[ObservableProperty] string _statusButtonText = "";
		[ObservableProperty] bool _showConnectionButton;

		public void ToggleConnection() => _connectionPresenter.ToggleConnection();
	}
}
