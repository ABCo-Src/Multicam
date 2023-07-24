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
    public partial class SwitcherButtonViewModel : ViewModelBase
    {
        public readonly Action ClickAction;
        public readonly int Id;
        public readonly ISwitcherMixBlockViewModel Parent;

        [ObservableProperty] string _text;
        [ObservableProperty] SwitcherButtonStatus _status;

        public SwitcherButtonViewModel(IServiceSource source, ISwitcherMixBlockViewModel parent)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            Parent = parent;
        }

        public SwitcherButtonViewModel(SwitcherBusInput input, bool isProgram, IServiceSource source, ISwitcherMixBlockViewModel parent) : this(source, parent)
        {
            Id = input.Id;
            Text = input.Name;
            ClickAction = isProgram ? Action.ProgramSwitch : Action.PreviewSwitch;
        }

        public SwitcherButtonViewModel(Action action, IServiceSource source, ISwitcherMixBlockViewModel parent) : this(source, parent)
        {
            
        }

        public enum Action
        {
            ProgramSwitch,
            PreviewSwitch,
            Cut,
            Auto
        }
    }
}
