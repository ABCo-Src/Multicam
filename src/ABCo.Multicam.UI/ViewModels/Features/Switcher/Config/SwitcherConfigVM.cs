﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;
using ABCo.Multicam.UI.Presenters.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherConfigVM : IParameteredService<ISwitcherConfigPresenter>, INotifyPropertyChanged
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
