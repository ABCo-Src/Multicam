using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public class SwitcherSpecs
    {
        public readonly IReadOnlyList<SwitcherMixBlock> MixBlocks;

        public SwitcherSpecs() => MixBlocks = Array.Empty<SwitcherMixBlock>();
        public SwitcherSpecs(params SwitcherMixBlock[] mixBlocks) => MixBlocks = mixBlocks;        

    }

    public class SwitcherMixBlock
    {
        public readonly IReadOnlyList<SwitcherBusInput> ProgramInputs;
        public readonly IReadOnlyList<SwitcherBusInput>? PreviewInputs;

        public SwitcherMixBlockType NativeType;

        public SwitcherMixBlock() => (NativeType, ProgramInputs, PreviewInputs) = (SwitcherMixBlockType.Unknown, Array.Empty<SwitcherBusInput>(), null);

        private SwitcherMixBlock(SwitcherBusInput[] programInputs) =>
            (NativeType, ProgramInputs, PreviewInputs) = (SwitcherMixBlockType.CutBus, programInputs, null);

        private SwitcherMixBlock(SwitcherBusInput[] programInputs, params SwitcherBusInput[] previewInputs) => 
            (NativeType, ProgramInputs, PreviewInputs) = (SwitcherMixBlockType.ProgramPreview, programInputs, previewInputs);

        public static SwitcherMixBlock NewCutBus(params SwitcherBusInput[] programInputs) => new(programInputs);
        public static SwitcherMixBlock NewProgPrev() => new(Array.Empty<SwitcherBusInput>(), Array.Empty<SwitcherBusInput>());
        public static SwitcherMixBlock NewProgPrev(SwitcherBusInput[] programInputs, params SwitcherBusInput[] previewInputs) => new(programInputs, previewInputs);
        public static SwitcherMixBlock NewProgPrevSameInputs(params SwitcherBusInput[] inputs) => new(inputs, inputs);
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
