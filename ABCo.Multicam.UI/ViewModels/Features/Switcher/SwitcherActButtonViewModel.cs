using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherActButtonViewModel { }
    public abstract class SwitcherActButtonViewModel : SwitcherButtonViewModel, ISwitcherActButtonViewModel
    {
        public SwitcherActButtonViewModel(NewViewModelInfo info, string text, IServiceSource source) : base(source, (ISwitcherMixBlockVM)info.Parent, text) { }
    }

    public interface ISwitcherCutButtonViewModel : ISwitcherActButtonViewModel { }
    public class SwitcherCutButtonViewModel : SwitcherActButtonViewModel, ISwitcherCutButtonViewModel
    {
        public SwitcherCutButtonViewModel(NewViewModelInfo info, IServiceSource source) : base(info, "Cut", source) { }
    }

    public interface ISwitcherAutoButtonViewModel : ISwitcherActButtonViewModel { }
    public class SwitcherAutoButtonViewModel : SwitcherActButtonViewModel, ISwitcherAutoButtonViewModel
    {
        public SwitcherAutoButtonViewModel(NewViewModelInfo info, IServiceSource source) : base(info, "Auto", source) { }
    }
}
