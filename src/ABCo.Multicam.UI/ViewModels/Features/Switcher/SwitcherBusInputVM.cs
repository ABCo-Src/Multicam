using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Enumerations;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherBusInputVM : INeedsInitialization<SwitcherBusInput, ISwitcherMixBlockVM>, ISwitcherButtonVM
    {
        SwitcherBusInput Base { get; }
        void SetHighlight(bool visible);
    }

    public abstract class SwitcherBusInputVM : SwitcherButtonVM, ISwitcherBusInputVM
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

    public interface ISwitcherProgramInputVM : ISwitcherBusInputVM { }
    public partial class SwitcherProgramInputVM : SwitcherBusInputVM, ISwitcherProgramInputVM
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

    public interface ISwitcherPreviewInputVM : ISwitcherBusInputVM { }
    public partial class SwitcherPreviewInputVM : SwitcherBusInputVM, ISwitcherPreviewInputVM
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