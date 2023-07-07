using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips.Switcher
{
    public partial class SwitcherButtonViewModel : ViewModelBase
    {
        public readonly ISwitcherMixBlockViewModel Parent;

        [ObservableProperty] string _text;
        [ObservableProperty] SwitcherButtonStatus _status;

        public SwitcherButtonViewModel(IServiceSource source, ISwitcherMixBlockViewModel parent, string text) 
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            Parent = parent;
            _text = text;
        }
    }
}
