using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips.Switcher
{
    public partial class SwitcherBusViewModel : ViewModelBase
    {
        public readonly ISwitcherStripViewModel Parent;

        [ObservableProperty] ObservableCollection<SwitcherInputButtonViewModel> _programInputs;
        [ObservableProperty] ObservableCollection<SwitcherInputButtonViewModel> _previewInputs;

        public SwitcherBusViewModel(IServiceSource source, ISwitcherStripViewModel parent)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            Parent = parent;
            ProgramInputs = new ObservableCollection<SwitcherInputButtonViewModel>()
            {
                new SwitcherInputButtonViewModel(),
                new SwitcherInputButtonViewModel(),
                new SwitcherInputButtonViewModel()
            };
            PreviewInputs = new ObservableCollection<SwitcherInputButtonViewModel>()
            {
                new SwitcherInputButtonViewModel(),
                new SwitcherInputButtonViewModel()
            };
        }
    }
}