using ABCo.Multicam.Core;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherActButtonVM : ISwitcherButtonVM, INeedsInitialization<ISwitcherMixBlockVM> { }

    public interface ISwitcherCutButtonVM : ISwitcherActButtonVM { }
    public class SwitcherCutButtonVM : SwitcherButtonVM, ISwitcherCutButtonVM
    {
        public SwitcherCutButtonVM() => Text = "Cut";
        public void FinishConstruction(ISwitcherMixBlockVM parent) => _parent = parent;
        public override void Click() => _parent.CutButtonPress();
    }

    public interface ISwitcherAutoButtonVM : ISwitcherActButtonVM { }
    public class SwitcherAutoButtonVM : SwitcherButtonVM, ISwitcherAutoButtonVM
    {
        public SwitcherAutoButtonVM() => Text = "Auto";
        public void FinishConstruction(ISwitcherMixBlockVM parent) => _parent = parent;

        public override void Click()
        {
            throw new NotImplementedException();
        }
    }
}
