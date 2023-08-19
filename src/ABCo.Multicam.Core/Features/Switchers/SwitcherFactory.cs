using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    // Creates a switcher from a given config
    public interface ISwitcherFactory 
    {
        ISwitcher GetSwitcher(SwitcherConfig config);
    }
}