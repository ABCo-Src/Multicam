using ABCo.Multicam.Core.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips
{
    /// <summary>
    /// Represents a strip currently running
    /// </summary>
    public interface IRunningStrip
    {
        
    }

    class DummyRunningStrip : ISwitcherRunningStrip { }
}
