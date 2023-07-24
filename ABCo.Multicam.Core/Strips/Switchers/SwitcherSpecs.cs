using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips.Switchers
{
    public class SwitcherSpecs
    {
        public readonly IReadOnlyList<SwitcherMixBlock> MixBlocks;

        public SwitcherSpecs() => MixBlocks = Array.Empty<SwitcherMixBlock>();
        public SwitcherSpecs(SwitcherMixBlock[] mixBlocks) => MixBlocks = mixBlocks;
    }

    public class SwitcherMixBlock
    {
        public readonly IReadOnlyList<SwitcherBusInput> ProgramInputs;
        public readonly IReadOnlyList<SwitcherBusInput>? PreviewInputs;

        public SwitcherBusInputType NativeType;

        public SwitcherMixBlock() => (NativeType, ProgramInputs, PreviewInputs) = (SwitcherBusInputType.Unknown, Array.Empty<SwitcherBusInput>(), null);
        public SwitcherMixBlock(SwitcherBusInputType nativeType, SwitcherBusInput[] programInputs, SwitcherBusInput[]? previewInputs) => (NativeType, ProgramInputs, PreviewInputs) = (nativeType, programInputs, previewInputs);
    }

    public class SwitcherBusInput
    {
        public int Id;
        public string Name;

        public SwitcherBusInput(int id, string name) => (Id, Name) = (id, name);
    }

    public enum SwitcherBusInputType
    {
        CutBus,
        PreviewProgram,
        Unknown
    }
}
