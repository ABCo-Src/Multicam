using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherActButtonViewModel : ISwitcherButtonViewModel
    {
        void FinishConstruction(ISwitcherMixBlockVM parent);
        void Click();
    }

    public interface ISwitcherCutButtonViewModel : ISwitcherActButtonViewModel { }
    public class SwitcherCutButtonViewModel : SwitcherButtonViewModel, ISwitcherCutButtonViewModel
    {
        public SwitcherCutButtonViewModel() => Text = "Cut";
        public void FinishConstruction(ISwitcherMixBlockVM parent) => _parent = parent;
        public override void Click() => _parent.CutButtonPress();
    }

    public interface ISwitcherAutoButtonViewModel : ISwitcherActButtonViewModel { }
    public class SwitcherAutoButtonViewModel : SwitcherButtonViewModel, ISwitcherAutoButtonViewModel
    {
        public SwitcherAutoButtonViewModel() => Text = "Auto";
        public void FinishConstruction(ISwitcherMixBlockVM parent) => _parent = parent;

        public override void Click()
        {
            throw new NotImplementedException();
        }
    }
}
