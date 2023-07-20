using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Switchers
{
    public struct SwitcherSpecs
    {
        public SwitcherMixBlock[] MixBlocks;
    }

    public struct SwitcherMixBlock
    {
        public SwitcherBusInput[] Inputs;
    }

    public struct SwitcherBusInput
    {
        public int Id;
        public SwitcherBusInputType Type;
    }

    public enum SwitcherBusInputType
    {
        CutBus,
        PreviewProgram,
        Unknown
    }
}
