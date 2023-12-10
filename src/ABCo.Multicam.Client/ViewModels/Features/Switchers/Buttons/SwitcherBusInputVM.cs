using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.Enumerations;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
	public interface ISwitcherBusInputVM : ISwitcherButtonVM
    {
		int BusId { get; }
		void UpdateState(MixBlockState state);
	}

    public abstract class SwitcherBusInputVM : SwitcherButtonVM
    {
		protected int _mixBlockIndex;
		public int BusId { get; }

		public SwitcherBusInputVM(Dispatched<ISwitcher> switcher, int mixBlockIndex, SwitcherBusInput info) : base(switcher, info.Name)
		{
			_mixBlockIndex = mixBlockIndex;
			BusId = info.Id;
		}
	}

    public interface ISwitcherProgramInputVM : ISwitcherBusInputVM { }
    public partial class SwitcherProgramInputVM : SwitcherBusInputVM, ISwitcherProgramInputVM
	{
		public SwitcherProgramInputVM(Dispatched<ISwitcher> switcher, int mixBlockIndex, SwitcherBusInput info) : base(switcher, mixBlockIndex, info) { }
        public override void Click() => _switcher.CallDispatched(f => f.SetProgram(_mixBlockIndex, BusId));
		public override void UpdateState(MixBlockState state) => Status = state.Prog == BusId ? SwitcherButtonStatus.ProgramActive : SwitcherButtonStatus.NeutralInactive;
	}

	public interface ISwitcherPreviewInputVM : ISwitcherBusInputVM { }
	public partial class SwitcherPreviewInputVM : SwitcherBusInputVM, ISwitcherPreviewInputVM
	{
		public SwitcherPreviewInputVM(Dispatched<ISwitcher> switcher, int mixBlockIndex, SwitcherBusInput info) : base(switcher, mixBlockIndex, info) { }
		public override void Click() => _switcher.CallDispatched(f => f.SetPreview(_mixBlockIndex, BusId));
		public override void UpdateState(MixBlockState state) => Status = state.Prev == BusId ? SwitcherButtonStatus.PreviewActive : SwitcherButtonStatus.NeutralInactive;
	}
}