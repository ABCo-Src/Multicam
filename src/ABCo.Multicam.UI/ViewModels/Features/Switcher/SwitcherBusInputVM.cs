using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Presenters.Features.Switcher;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherBusInputVM : IParameteredService<ISwitcherMixBlocksPresenter, int, int>, ISwitcherButtonVM
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
		public static ISwitcherProgramInputVM New(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex, int busIndex, IServiceSource servSource) =>
			new SwitcherProgramInputVM(presenter, mixBlockIndex, busIndex);

		public SwitcherProgramInputVM(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex, int busIndex) : base(presenter, mixBlockIndex, busIndex) { }
        public override void Click() => _presenter.SetProgram(_mixBlockIndex, BusId);
    }

	public interface ISwitcherPreviewInputVM : ISwitcherBusInputVM { }
	public partial class SwitcherPreviewInputVM : SwitcherBusInputVM, ISwitcherPreviewInputVM
	{
		public static ISwitcherPreviewInputVM New(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex, int busIndex, IServiceSource servSource) =>
			new SwitcherPreviewInputVM(presenter, mixBlockIndex, busIndex);
		public SwitcherPreviewInputVM(ISwitcherMixBlocksPresenter presenter, int mixBlockIndex, int busIndex) : base(presenter, mixBlockIndex, busIndex) { }
		public override void Click() => _presenter.SetPreview(_mixBlockIndex, BusId);
	}
}