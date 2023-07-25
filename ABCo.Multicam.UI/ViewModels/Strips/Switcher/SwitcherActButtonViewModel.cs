using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips.Switcher
{
    public class SwitcherActButtonViewModel : SwitcherButtonViewModel
    {
        public readonly Type Action;
        public SwitcherActButtonViewModel(Type act, IServiceSource source, ISwitcherMixBlockViewModel parent) : base(source, parent, act == Type.Cut ? "Cut" : "Auto")
            => Action = act;

        public enum Type
        {
            Cut,
            Auto
        }
    }
}
