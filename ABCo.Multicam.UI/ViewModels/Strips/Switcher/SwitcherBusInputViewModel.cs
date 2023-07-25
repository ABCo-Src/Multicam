using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips.Switchers;
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
    public partial class SwitcherBusInputViewModel : ViewModelBase
    {
        public readonly SwitcherBusInput Base;
        public readonly bool IsProgram;
        public readonly ISwitcherMixBlockViewModel Parent;

        public string Text => Base.Name;

        [ObservableProperty] SwitcherButtonStatus _status;

        public SwitcherBusInputViewModel(SwitcherBusInput input, bool isProgram, IServiceSource source, ISwitcherMixBlockViewModel parent)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            Parent = parent;
            Base = input;
            IsProgram = isProgram;
        }
    }
}
