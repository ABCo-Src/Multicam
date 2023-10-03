using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
	public interface ISwitcherConfigVM : IClientService<ISwitcherConfigPresenter>, INotifyPropertyChanged
    {
		string[] Items { get; }
        string SelectedItem { get; set; }
		ISwitcherSpecificConfigVM? CurrentConfig { get; set; }
        void UpdateSelectedItem();
	}

    public interface ISwitcherSpecificConfigVM
    {

    }

    public partial class SwitcherConfigVM : ViewModelBase, ISwitcherConfigVM
    {
		readonly ISwitcherConfigPresenter _presenter;

        public string[] Items => new string[]
        {
            "Dummy",
            "ATEM"
        };

        [ObservableProperty] string _selectedItem = "Dummy";
        [ObservableProperty] ISwitcherSpecificConfigVM? _currentConfig;

        public SwitcherConfigVM(ISwitcherConfigPresenter presenter)
        {
            _presenter = presenter;
		}

        public void UpdateSelectedItem()
        {
            _presenter.SelectedChanged();
        }
    }
}
