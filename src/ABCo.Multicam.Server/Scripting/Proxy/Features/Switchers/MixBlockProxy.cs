using ABCo.Multicam.Server.Features.Switchers;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Proxy.Features.Switchers
{
	[MoonSharpUserData]
	public class MixBlockProxy
	{
		readonly ISwitcher _switcher;
		readonly int _mixBlock;

		public MixBlockProxy(ISwitcher switcher, int mixBlock)
		{
			_switcher = switcher;
			_mixBlock = mixBlock;

			if (_mixBlock >= _switcher.SpecsInfo.State.Count) 
				throw new Exception("Attempt to access a mix-block that does not exist. The switcher may just be disconnected.");
		}

		public BusProxy Prog => new(_switcher, _mixBlock, true);
		public BusProxy Prev => new(_switcher, _mixBlock, false);

		public override string ToString() => $"Switchers[\"{_switcher.Name}\"].MixBlocks[{_mixBlock + 1}]";
	}
}
