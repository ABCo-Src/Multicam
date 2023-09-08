﻿using ABCo.Multicam.UI.Enumerations;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherButtonVM : INotifyPropertyChanged
    {
        string Text { get; set; }
        SwitcherButtonStatus Status { get; set; }
        void Click();
    }

    public abstract partial class SwitcherButtonVM : ViewModelBase, ISwitcherButtonVM
    {
        [ObservableProperty] string _text = "";
        [ObservableProperty] SwitcherButtonStatus _status;

        public abstract void Click();
    }
}
