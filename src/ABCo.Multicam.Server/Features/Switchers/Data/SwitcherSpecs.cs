using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features.Switchers
{
    public class SwitcherSpecs : ServerData
    {
        public readonly bool CanChangeConnection;
        public readonly IReadOnlyList<SwitcherMixBlock> MixBlocks;

        public SwitcherSpecs() => MixBlocks = Array.Empty<SwitcherMixBlock>();
        public SwitcherSpecs(params SwitcherMixBlock[] mixBlocks) => MixBlocks = mixBlocks;
		public SwitcherSpecs(bool canChangeConnection, params SwitcherMixBlock[] mixBlocks) => (CanChangeConnection, MixBlocks) = (canChangeConnection, mixBlocks);
	}

    public class SwitcherMixBlock
    {
        public readonly IReadOnlyList<SwitcherBusInput> ProgramInputs;
        public readonly IReadOnlyList<SwitcherBusInput> PreviewInputs;

        public readonly SwitcherMixBlockType NativeType;
        public readonly SwitcherMixBlockFeatures SupportedFeatures;

        public SwitcherMixBlock() => (NativeType, ProgramInputs, PreviewInputs, SupportedFeatures) = (SwitcherMixBlockType.Unknown, Array.Empty<SwitcherBusInput>(), Array.Empty<SwitcherBusInput>(), new());

        private SwitcherMixBlock(SwitcherMixBlockFeatures features, SwitcherBusInput[] programInputs) =>
            (NativeType, ProgramInputs, PreviewInputs, SupportedFeatures) = (SwitcherMixBlockType.CutBus, programInputs, Array.Empty<SwitcherBusInput>(), features);

        private SwitcherMixBlock(SwitcherMixBlockFeatures features, SwitcherBusInput[] programInputs, params SwitcherBusInput[] previewInputs) => 
            (NativeType, ProgramInputs, PreviewInputs, SupportedFeatures) = (SwitcherMixBlockType.ProgramPreview, programInputs, previewInputs, features);

        public static SwitcherMixBlock NewCutBus(SwitcherMixBlockFeatures features, params SwitcherBusInput[] programInputs) => new(features, programInputs);
        public static SwitcherMixBlock NewProgPrev(SwitcherMixBlockFeatures features) => new(features, Array.Empty<SwitcherBusInput>(), Array.Empty<SwitcherBusInput>());
        public static SwitcherMixBlock NewProgPrev(SwitcherMixBlockFeatures features, SwitcherBusInput[] programInputs, params SwitcherBusInput[] previewInputs) => new(features, programInputs, previewInputs);
        public static SwitcherMixBlock NewProgPrevSameInputs(SwitcherMixBlockFeatures features, params SwitcherBusInput[] inputs) => new(features, inputs, inputs);
    }

    public class SwitcherMixBlockFeatures
    {
        // States that the mix block currently supports changing the program bus directly (with **no** applied fade).
        // (*Getting* the program bus should always be possible)
        public bool SupportsDirectProgramModification { get; init; } = false;

        // States that the mix block has a preview bus currently and it can be changed (presumably with no fade applied...)
        public bool SupportsDirectPreviewAccess { get; init; } = false;

        // States that the mix block currently supports the program-preview-swapping "Cut" operation.
        public bool SupportsCutAction { get; init; } = false;

        // States that the mix block currently supports the program-preview-swapping "Auto" operation.
        public bool SupportsAutoAction { get; init; } = false;

        // States that the mix block currently supports changing the cut bus mode (between Cut and Auto).
        public bool SupportsCutBusModeChanging { get; init; } = false;

        // States that the mix block currently supports the input being switched *with the cut bus mode applied to it*
        public bool SupportsCutBusSwitching { get; init; } = false;

        // States that the mix block currently supports the "Cut" mode of the cut bus.
        public bool SupportsCutBusCutMode { get; init; } = false;

        // States that the mix block currently supports the "Auto" mode of the cut bus.
        public bool SupportsCutBusAutoMode { get; init; } = false;

        public SwitcherMixBlockFeatures() { }
        public SwitcherMixBlockFeatures(
            bool supportsDirectProgramModification = false,
            bool supportsDirectPreviewAccess = false,
            bool supportsCutAction = false,
            bool supportsAutoAction = false,
            bool supportsCutBusModeChanging = false,
            bool supportsCutBusSwitching = false,
            bool supportsCutBusCutMode = false,
            bool supportsCutBusAutoMode = false)
        {
            SupportsDirectProgramModification = supportsDirectProgramModification;
            SupportsDirectPreviewAccess = supportsDirectPreviewAccess;
            SupportsCutAction = supportsCutAction;
            SupportsAutoAction = supportsAutoAction;
            SupportsCutBusModeChanging = supportsCutBusModeChanging;
            SupportsCutBusSwitching = supportsCutBusSwitching;
            SupportsCutBusCutMode = supportsCutBusCutMode;
            SupportsCutBusAutoMode = supportsCutBusAutoMode;
        }
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
