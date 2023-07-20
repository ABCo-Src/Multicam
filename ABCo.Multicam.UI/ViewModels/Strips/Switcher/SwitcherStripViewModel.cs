using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Switchers;
using ABCo.Multicam.Core.Switchers.Types;
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
        ISwitcher _switcher;

        [ObservableProperty] ObservableCollection<SwitcherMixBlockViewModel> _mixBlocks;

        public SwitcherStripViewModel(IServiceSource serviceSource, IProjectStripsViewModel parent) : base(serviceSource, parent)
        {
            _mixBlocks = new ObservableCollection<SwitcherMixBlockViewModel>() { new SwitcherMixBlockViewModel(serviceSource, this), new SwitcherMixBlockViewModel(serviceSource, this) };
            _switcher = serviceSource.Get<IDummySwitcher>();
        }

        public override StripViewType ContentView => StripViewType.Switcher;
    }
}