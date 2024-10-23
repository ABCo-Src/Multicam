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
    public class SwitcherProxy
    {
        ISwitcher _switcher;

        // TODO: When adding globals, make sure there's some disposale 
        public SwitcherProxy(ISwitcher switcher)
        {
            _switcher = switcher;
        }

        public MixBlockProxy[] MixBlocks
        {
            get
            {
                var arr = new MixBlockProxy[_switcher.SpecsInfo.Specs.MixBlocks.Count];
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = new MixBlockProxy(_switcher, i);
                return arr;
            }
        }

        public override string ToString() => $"Switchers[\"{_switcher.Name}\"]";
    }
}
