using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
	public class SwitcherState : ServerData
	{
		public MixBlockState[] Data { get; }
		public SwitcherState(MixBlockState[] data) => Data = data;
	}

	public record struct MixBlockState(int Prog, int Prev);
}
