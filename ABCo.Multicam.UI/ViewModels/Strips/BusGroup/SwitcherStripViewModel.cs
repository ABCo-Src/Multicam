using ABCo.Multicam.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips.BusGroup
{
    public interface ISwitcherStripViewModel { }
    public partial class SwitcherStripViewModel : StripViewModel, ISwitcherStripViewModel
    {
        [ObservableProperty] ObservableCollection<SwitcherButtonSetViewModel> _buttonColumns;
        public SwitcherStripViewModel(IServiceSource serviceSource, IProjectStripsViewModel parent) : base(serviceSource, parent) => _buttonColumns = new ObservableCollection<SwitcherButtonSetViewModel>();
    }
}