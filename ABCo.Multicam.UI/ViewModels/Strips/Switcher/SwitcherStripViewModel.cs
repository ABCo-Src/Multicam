﻿using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips.Switcher
{
    public interface ISwitcherStripViewModel { }
    public partial class SwitcherStripViewModel : StripViewModel, ISwitcherStripViewModel
    {
        [ObservableProperty] ObservableCollection<SwitcherBusViewModel> _buses;
        public SwitcherStripViewModel(IServiceSource serviceSource, IProjectStripsViewModel parent) : base(serviceSource, parent) => _buses = new ObservableCollection<SwitcherBusViewModel>() { new SwitcherBusViewModel(serviceSource, this) };

        public override StripViewType ContentView => StripViewType.Switcher;
    }
}