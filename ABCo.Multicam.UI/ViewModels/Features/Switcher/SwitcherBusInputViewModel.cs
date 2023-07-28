using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public partial class SwitcherBusInputViewModel : SwitcherButtonViewModel
    {
        public readonly SwitcherBusInput Base;
        public readonly bool IsProgram;

        public SwitcherBusInputViewModel(SwitcherBusInput input, bool isProgram, IServiceSource source, ISwitcherMixBlockViewModel parent) : base(source, parent, input.Name)
        {
            Text = input.Name;
            Base = input;
            IsProgram = isProgram;
        }
    }
}
