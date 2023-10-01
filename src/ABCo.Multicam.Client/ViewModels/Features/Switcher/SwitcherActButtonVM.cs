using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features.Switcher;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
	public interface ISwitcherActButtonVM : ISwitcherButtonVM, IClientService<ISwitcherMixBlocksPresenter, int> { }

	public abstract class SwitcherActButtonVM : SwitcherButtonVM
	{
        protected ISwitcherMixBlocksPresenter _presenter;
        protected int _mixBlockIndex;

		public SwitcherActButtonVM(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex)
		{
            _presenter = presenter;
            _mixBlockIndex = mixBlockIndex;
		}
	}

	public interface ISwitcherCutButtonVM : ISwitcherActButtonVM { }
    public class SwitcherCutButtonVM : SwitcherActButtonVM, ISwitcherCutButtonVM
    {
		public SwitcherCutButtonVM(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex) : base(presenter, mixBlockIndex) => Text = "Cut";
		public override void Click() => _presenter.Cut(_mixBlockIndex);
    }
}
