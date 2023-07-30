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
    public interface ISwitcherBusInputViewModel { }
    public abstract class SwitcherBusInputViewModel : SwitcherButtonViewModel, ISwitcherBusInputViewModel
    {
        public readonly SwitcherBusInput Base;
        public readonly bool IsProgram;

        public SwitcherBusInputViewModel(NewViewModelInfo info, bool isProgram, IServiceSource source) : base(source, (ISwitcherMixBlockVM)info.Parent, ((SwitcherBusInput)info.Model!).Name)
        {
            Base = (SwitcherBusInput)info.Model;
            IsProgram = isProgram;
        }
    }

    public interface ISwitcherProgramInputViewModel : ISwitcherBusInputViewModel { }
    public partial class SwitcherProgramInputViewModel : SwitcherBusInputViewModel, ISwitcherProgramInputViewModel
    {
        public SwitcherProgramInputViewModel(NewViewModelInfo info, IServiceSource source) : base(info, true, source) { }
    }

    public interface ISwitcherPreviewInputViewModel : ISwitcherBusInputViewModel { }
    public partial class SwitcherPreviewInputViewModel : SwitcherBusInputViewModel, ISwitcherPreviewInputViewModel
    {
        public SwitcherPreviewInputViewModel(NewViewModelInfo info, IServiceSource source) : base(info, false, source) { }
    }
}