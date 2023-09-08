using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Presenters.Features.Switcher;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherActButtonVM : ISwitcherButtonVM, INeedsInitialization<ISwitcherMixBlocksPresenter, int> { }

	public abstract class SwitcherActButtonVM : SwitcherButtonVM
	{
        protected ISwitcherMixBlocksPresenter _presenter = null!;
        protected int _mixBlockIndex;

		public void FinishConstruction(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex)
		{
            _presenter = presenter;
            _mixBlockIndex = mixBlockIndex;
		}
	}

	public interface ISwitcherCutButtonVM : ISwitcherActButtonVM { }
    public class SwitcherCutButtonVM : SwitcherActButtonVM, ISwitcherCutButtonVM
    {
        public SwitcherCutButtonVM() => Text = "Cut";
		public override void Click() => _presenter.Cut(_mixBlockIndex);
    }
}
