using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips.Switchers
{
    public interface ISwitcherRunningStrip : IRunningStrip 
    { 
        ISwitcher Switcher { get; }
    }

    /// <summary>
    /// Represents an active switcher in the project. Provides caching, preview bus emulation and some event handling over the raw ISwitcher.
    /// </summary>
    public class SwitcherRunningStrip : ISwitcherRunningStrip
    {
        public ISwitcher Switcher { get; }
        public SwitcherSpecs SwitcherSpecs { get; }

        public SwitcherRunningStrip(IDummySwitcher switcher)
        {
            Switcher = switcher;
        }

        public void Dispose() => Switcher.Dispose();
    }
}
