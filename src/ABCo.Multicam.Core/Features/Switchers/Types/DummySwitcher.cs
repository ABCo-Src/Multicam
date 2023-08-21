using LightInject;

namespace ABCo.Multicam.Core.Features.Switchers.Types
{
    public interface IDummySwitcher : ISwitcher, INeedsInitialization<DummySwitcherConfig> { }

    public class DummySwitcherConfig : SwitcherConfig
    {
        public override SwitcherType Type => SwitcherType.Dummy;
        public int[] MixBlocks { get; }

        public DummySwitcherConfig(params int[] mixBlocks) => MixBlocks = mixBlocks;
    }

    public class DummySwitcher : IDummySwitcher
    {
        SwitcherSpecs _specs = null!;
        MixBlockState[] _states = null!;
        ISwitcherEventHandler? _eventHandler = null!;

        public void FinishConstruction(DummySwitcherConfig config)
        {
            _specs = CreateSpecsFrom(config.MixBlocks);

            // Create new state, starting at 1
            _states = new MixBlockState[_specs.MixBlocks.Count];
            Array.Fill(_states, new MixBlockState(1, 1));
        }

        public void SetEventHandler(ISwitcherEventHandler? eventHandler) => _eventHandler = eventHandler;

        public SwitcherSpecs RefreshSpecs()
        {
            _eventHandler?.OnSpecsChange(_specs);
            return _specs;
        }

        public static SwitcherSpecs CreateSpecsFrom(int[] mixBlocks)
        {
            var mixBlocksArray = new SwitcherMixBlock[mixBlocks.Length];

            for (int i = 0; i < mixBlocks.Length; i++)
            {
                var programArray = new SwitcherBusInput[mixBlocks[i]];

                for (int j = 0; j < programArray.Length; j++)
                    programArray[j] = new SwitcherBusInput(j + 1, "Cam " + (j + 1));

                mixBlocksArray[i] = SwitcherMixBlock.NewProgPrevSameInputs(CreateFeatures(), programArray);
            }

            return new SwitcherSpecs(mixBlocksArray);
        }

        static SwitcherMixBlockFeatures CreateFeatures() => new()
        {
            SupportsDirectProgramModification = true,
            SupportsDirectPreviewAccess = true,
            SupportsCutAction = false,
            SupportsAutoAction = true,
            SupportsCutBusModeChanging = false,
            SupportsCutBusSwitching = false,
            SupportsCutBusCutMode = false,
            SupportsCutBusAutoMode = false
        };

        public bool IsConnected => true;
        public SwitcherConfig ConnectionConfig => throw new NotImplementedException();

        public Task ConnectAsync() => Task.CompletedTask;
        public Task DisconnectAsync() => Task.CompletedTask;

        public int RefreshProgram(int mixBlock)
        {
            ValidateMixBlock(mixBlock);
            _eventHandler?.OnProgramChangeFinish(new SwitcherProgramChangeInfo(mixBlock, 0, _states[mixBlock].Program, null));
            return 0;
        }

        public void RefreshPreview(int mixBlock)
        {
            ValidateMixBlock(mixBlock);
            _eventHandler?.OnPreviewChangeFinish(new SwitcherPreviewChangeInfo(mixBlock, _states[mixBlock].Preview, null));
        }

        public void SendProgramValue(int mixBlock, int newValue)
        {
            ValidateMixBlock(mixBlock);
            ValidateInput(mixBlock, newValue);

            _states[mixBlock].Program = newValue;

            _eventHandler?.OnProgramChangeFinish(new SwitcherProgramChangeInfo(mixBlock, 0, _states[mixBlock].Program, null));
        }

        void ValidateInput(int mixBlock, int newValue)
        {
            for (int i = 0; i < _specs.MixBlocks[mixBlock].ProgramInputs.Count; i++)
                if (_specs.MixBlocks[mixBlock].ProgramInputs[i].Id == newValue)
                    return;

            throw new ArgumentException("Invalid input ID given to DummySwitcher");
        }

        public void SendPreviewValue(int mixBlock, int newValue)
        {
            ValidateMixBlock(mixBlock);
            ValidateInput(mixBlock, newValue);

            _states[mixBlock].Preview = newValue;

            _eventHandler?.OnPreviewChangeFinish(new SwitcherPreviewChangeInfo(mixBlock, _states[mixBlock].Preview, null));
        }

        void ValidateMixBlock(int mixBlock)
        {
            if (mixBlock < 0 || mixBlock >= _specs.MixBlocks.Count) 
                throw new ArgumentException("Invalid mix block given to DummySwitcher");
        }

        public void Cut(int mixBlockIdx) => throw new NotImplementedException();
        public void Dispose() { }

        public void SetCutBus(int mixBlock, int newVal) => throw new NotImplementedException();
        public void SetCutBusMode(CutBusMode mode) => throw new NotImplementedException();
        public CutBusMode GetCutBusMode(int mixBlock) => throw new NotImplementedException();
        public void SetCutBusMode(int mixBlock, CutBusMode mode) => throw new NotImplementedException();

        public record struct MixBlockState(int Program, int Preview);
    }
}