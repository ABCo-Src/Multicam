namespace ABCo.Multicam.Server.Features.Switchers.Data.Config
{
	public class VirtualSwitcherConfig : SwitcherConfig
	{
		public override SwitcherType Type => SwitcherType.Virtual;

		public int[] MixBlocks { get; }
		public VirtualSwitcherConfig(params int[] mixBlocks) => MixBlocks = mixBlocks;
	}
}
