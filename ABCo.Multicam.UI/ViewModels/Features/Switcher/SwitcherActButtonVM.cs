using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherActButtonVM : ISwitcherButtonVM
    {
        void FinishConstruction(ISwitcherMixBlockVM parent);
        void Click();
    }

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
