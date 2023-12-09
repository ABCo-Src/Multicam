using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features.Switchers;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
    public interface ISwitcherBusInputVM : IClientService<ISwitcherMixBlocksPresenter, int, int>, ISwitcherButtonVM
    {
		int BusId { get; }
	}

    public abstract class SwitcherBusInputVM : SwitcherButtonVM
    {
		protected ISwitcherMixBlocksPresenter _presenter;
		protected int _mixBlockIndex;
		public int BusId { get; private set; }

		public SwitcherBusInputVM(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex, int busIndex)
		{
			_presenter = presenter;
			_mixBlockIndex = mixBlockIndex;
			BusId = busIndex;
		}
	}

    public interface ISwitcherProgramInputVM : ISwitcherBusInputVM { }
    public partial class SwitcherProgramInputVM : SwitcherBusInputVM, ISwitcherProgramInputVM
	{
		public SwitcherProgramInputVM(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex, int busIndex) : base(presenter, mixBlockIndex, busIndex) { }
        public override void Click() => _presenter.SetProgram(_mixBlockIndex, BusId);
    }

	public interface ISwitcherPreviewInputVM : ISwitcherBusInputVM { }
	public partial class SwitcherPreviewInputVM : SwitcherBusInputVM, ISwitcherPreviewInputVM
	{
		public SwitcherPreviewInputVM(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex, int busIndex) : base(presenter, mixBlockIndex, busIndex) { }
		public override void Click() => _presenter.SetPreview(_mixBlockIndex, BusId);
	}
}