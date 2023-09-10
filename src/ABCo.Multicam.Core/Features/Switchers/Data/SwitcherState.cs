using ABCo.Multicam.Core.Features.Data;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
	public class SwitcherState : FeatureData
	{
		public override int DataId => SwitcherDataSpecs.STATE;

		public MixBlockState[] Data { get; }
		public SwitcherState(MixBlockState[] data) => Data = data;
	}

	public record struct MixBlockState(int Prog, int Prev);
}
