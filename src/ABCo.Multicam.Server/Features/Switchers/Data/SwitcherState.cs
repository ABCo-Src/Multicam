using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting;

namespace ABCo.Multicam.Server.Features.Switchers.Data
{
    public class SwitcherState : ServerData
	{
		public MixBlockState[] Data { get; }
		public SwitcherState(MixBlockState[] data) => Data = data;
	}

	public record struct MixBlockState(int Prog, int Prev);
}
