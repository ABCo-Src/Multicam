using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips.Switchers
{
    public struct SwitcherSpecs
    {
        public readonly IReadOnlyList<SwitcherMixBlock> MixBlocks;

        public SwitcherSpecs(SwitcherMixBlock[] mixBlocks) => MixBlocks = mixBlocks;
    }

    public struct SwitcherMixBlock
    {
        public readonly IReadOnlyList<SwitcherBusInput> ProgramInputs;
        public readonly IReadOnlyList<SwitcherBusInput>? PreviewInputs;

        public SwitcherBusInputType NativeType;

        public SwitcherMixBlock(SwitcherBusInputType nativeType, SwitcherBusInput[] programInputs, SwitcherBusInput[]? previewInputs) => (NativeType, ProgramInputs, PreviewInputs) = (nativeType, programInputs, previewInputs);
    }

    public struct SwitcherBusInput
    {
        public int Id;

        public SwitcherBusInput(int id) => Id = id;
    }

    public enum SwitcherBusInputType
    {
        CutBus,
        PreviewProgram,
        Unknown
    }
}
