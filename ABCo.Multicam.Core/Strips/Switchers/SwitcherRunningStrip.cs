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
        SwitcherSpecs SwitcherSpecs { get; }
    }

    /// <summary>
    /// Represents an active switcher in the project. Provides caching, preview bus emulation and some event handling over the raw ISwitcher.
    /// </summary>
    public class SwitcherRunningStrip : ISwitcherRunningStrip
    {
        ISwitcher _rawSwitcher;
        public SwitcherSpecs SwitcherSpecs { get; private set; }

        public SwitcherRunningStrip(IDummySwitcher switcher)
        {
            _rawSwitcher = switcher;
            SwitcherSpecs = switcher.ReceiveSpecs();
        }
        
        public void Dispose() => _rawSwitcher.Dispose();
    }
}
