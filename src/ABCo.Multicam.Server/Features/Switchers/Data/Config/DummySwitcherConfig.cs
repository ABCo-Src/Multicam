using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Data.Config
{
	public class DummySwitcherConfig : SwitcherConfig
	{
		public override SwitcherType Type => SwitcherType.Dummy;

		public int[] MixBlocks { get; }
		public DummySwitcherConfig(params int[] mixBlocks) => MixBlocks = mixBlocks;
	}
}
