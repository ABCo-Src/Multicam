using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips.Switchers
{
    public interface ISwitcherRunningStrip : IRunningStrip { }

    /// <summary>
    /// Represents an active switcher in the project. 
    /// This is for the most part an ISwitcher factory that offers up a hot-swappable ISwitcher, most of the real magic happens in the viewmodels and the ISwitcher.
    /// </summary>
    public class SwitcherRunningStrip : ISwitcherRunningStrip
    {
        public ISwitcher Switcher { get; }

        public SwitcherRunningStrip(IDummySwitcher switcher)
        {
            Switcher = switcher;
        }

        public void Dispose() => Switcher.Dispose();
    }
}
