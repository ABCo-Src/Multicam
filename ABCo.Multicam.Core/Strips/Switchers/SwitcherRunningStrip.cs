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
    public class SwitcherRunningStrip : ISwitcherRunningStrip
    {
        public SwitcherRunningStrip(IDummySwitcher switcher)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
