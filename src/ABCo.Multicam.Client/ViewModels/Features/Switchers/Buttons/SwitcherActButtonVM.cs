using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Client.Enumerations;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
    public interface ISwitcherActButtonVM : ISwitcherButtonVM { }

    public abstract class SwitcherActButtonVM : SwitcherButtonVM
    {
        protected int _mixBlockIndex;

        public SwitcherActButtonVM(Dispatched<ISwitcher> switcher, int mixBlockIndex, string text) : base(switcher, text) => _mixBlockIndex = mixBlockIndex;
    }

    public interface ISwitcherCutButtonVM : ISwitcherActButtonVM { }
    public class SwitcherCutButtonVM : SwitcherActButtonVM, ISwitcherCutButtonVM
    {
        public SwitcherCutButtonVM(Dispatched<ISwitcher> switcher, int mixBlockIndex) : base(switcher, mixBlockIndex, "Cut") { }
        public override void Click() => _switcher.CallDispatched(s => s.Cut(_mixBlockIndex));
        public override void UpdateState(MixBlockState state) => Status = SwitcherButtonStatus.NeutralActive;
    }
}
