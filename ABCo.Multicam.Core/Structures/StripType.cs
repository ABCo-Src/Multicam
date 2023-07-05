using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Structures
{
    public struct StripType
    {
        public StripTypes Type;
    }

    public enum StripTypes
    {
        // v1 features
        BusGroup,
        SwitcherLink,
        Tally,
        Logger,
        Generator
    }
}