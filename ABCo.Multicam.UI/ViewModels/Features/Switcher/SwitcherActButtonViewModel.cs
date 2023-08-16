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
    public interface ISwitcherActButtonViewModel : ISwitcherButtonViewModel
    {
        void FinishConstruction(ISwitcherMixBlockVM parent);
        void Click();
    }

    public interface ISwitcherCutButtonViewModel : ISwitcherActButtonViewModel { }
    public class SwitcherCutButtonViewModel : SwitcherButtonViewModel, ISwitcherCutButtonViewModel
    {
        string creationStackTrace;
        public SwitcherCutButtonViewModel()
        {
            creationStackTrace = Environment.StackTrace;
            Text = "Cut";
        }

        public void FinishConstruction(ISwitcherMixBlockVM parent)
        {
            object obj = this;
            IntPtr val = Unsafe.As<object, IntPtr>(ref obj);
            _parent = parent;
        }

        public override void Click()
        {
            object obj = this;
            IntPtr val = Unsafe.As<object, IntPtr>(ref obj);
            _parent.CutButtonPress();
        }
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
