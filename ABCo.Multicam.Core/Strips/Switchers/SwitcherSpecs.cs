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

        public SwitcherMixBlockType NativeType;

        public SwitcherMixBlock() => (NativeType, ProgramInputs, PreviewInputs) = (SwitcherMixBlockType.Unknown, Array.Empty<SwitcherBusInput>(), null);
        public SwitcherMixBlock(SwitcherMixBlockType nativeType, SwitcherBusInput[] programInputs, SwitcherBusInput[]? previewInputs) => (NativeType, ProgramInputs, PreviewInputs) = (nativeType, programInputs, previewInputs);
    }

    public class SwitcherBusInput
    {
        public readonly int Id;
        public readonly string Name;

        public SwitcherBusInput() { }
        public SwitcherBusInput(int id, string name) => (Id, Name) = (id, name);
    }

    public enum SwitcherMixBlockType
    {
        CutBus,
        ProgramPreview,
        Unknown
    }
}
