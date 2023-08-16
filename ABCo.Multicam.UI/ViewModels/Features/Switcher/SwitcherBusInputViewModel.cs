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
    public interface ISwitcherBusInputViewModel 
    {
        SwitcherBusInput Base { get; }
        void FinishConstruction(SwitcherBusInput busInput, ISwitcherMixBlockVM parent);
        void SetHighlight(bool visible);
    }

    public abstract class SwitcherBusInputViewModel : SwitcherButtonViewModel, ISwitcherBusInputViewModel
    {
        public SwitcherBusInput Base { get; private set; } = null!;

        public void FinishConstruction(SwitcherBusInput busInput, ISwitcherMixBlockVM parent)
        {
            Base = busInput;
            Text = busInput.Name;
            _parent = parent;
        }

        public abstract void SetHighlight(bool visible);
    }

    public interface ISwitcherProgramInputViewModel : ISwitcherBusInputViewModel { }
    public partial class SwitcherProgramInputViewModel : SwitcherBusInputViewModel, ISwitcherProgramInputViewModel
    {
        public override void SetHighlight(bool visible)
        {
            if (visible)
                Status = SwitcherButtonStatus.ProgramActive;
            else
                Status = SwitcherButtonStatus.NeutralInactive;
        }

        public override void Click() => _parent.SetProgram(Base.Id);
    }

    public interface ISwitcherPreviewInputViewModel : ISwitcherBusInputViewModel { }
    public partial class SwitcherPreviewInputViewModel : SwitcherBusInputViewModel, ISwitcherPreviewInputViewModel
    {
        public override void SetHighlight(bool visible)
        {
            if (visible)
                Status = SwitcherButtonStatus.PreviewActive;
            else
                Status = SwitcherButtonStatus.NeutralInactive;
        }

        public override void Click() => _parent.SetPreview(Base.Id);
    }
}