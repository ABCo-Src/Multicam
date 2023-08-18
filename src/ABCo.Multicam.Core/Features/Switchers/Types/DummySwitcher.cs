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
        Action<SwitcherBusChangeInfo>? _busChangeFinishCallback;

        public DummySwitcher()
        {
            (_specs, _states) = (null!, null!); // Assigned by UpdateSpecs
        }

        public void FinishConstruction(DummySwitcherConfig config)
        {
            _specs = CreateSpecsFrom(config.MixBlocks);

            // Create new state, starting at 1
            _states = new MixBlockState[_specs.MixBlocks.Count];
            Array.Fill(_states, new MixBlockState(1, 1));
        }

        public SwitcherSpecs ReceiveSpecs() => _specs;

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

        public int ReceiveValue(int mixBlock, int bus)
        {
            ValidateMixBlockAndBus(mixBlock, bus);
            return bus == 0 ? _states[mixBlock].Program : _states[mixBlock].Preview;
        }

        public void PostValue(int mixBlock, int bus, int newValue)
        {
            // Validate mixBlock and bus
            ValidateMixBlockAndBus(mixBlock, bus);

            // Validate input
            bool found = false;
            for (int i = 0; i < _specs.MixBlocks[mixBlock].ProgramInputs.Count; i++)
                if (_specs.MixBlocks[mixBlock].ProgramInputs[i].Id == newValue)
                {
                    found = true;
                    break;
                }

            if (!found) throw new ArgumentException("Invalid input ID given to DummySwitcher");

            if (bus == 0)
                _states[mixBlock].Program = newValue;
            else
                _states[mixBlock].Preview = newValue;

            _busChangeFinishCallback?.Invoke(new SwitcherBusChangeInfo(true, mixBlock, bus, newValue, null));
        }

        void ValidateMixBlockAndBus(int mixBlock, int bus)
        {
            // Validate mix block
            if (mixBlock < 0 || mixBlock >= _specs.MixBlocks.Count) throw new ArgumentException("Invalid mix block given to DummySwitcher");

            // Validate bus
            if (bus == 0) return;
            if (bus == 1 && _specs.MixBlocks[mixBlock].NativeType == SwitcherMixBlockType.ProgramPreview) return;

            throw new ArgumentException("Invalid bus given to DummySwitcher");
        }

        public void Cut(int mixBlockIdx) => throw new NotImplementedException();
        public void Dispose() { }

        public void SetOnBusChangeFinishCall(Action<SwitcherBusChangeInfo>? callback) => _busChangeFinishCallback = callback;

        public void SetCutBus(int mixBlock, int newVal) => throw new NotImplementedException();
        public void SetCutBusMode(CutBusMode mode) => throw new NotImplementedException();
        public CutBusMode GetCutBusMode(int mixBlock) => throw new NotImplementedException();
        public void SetCutBusMode(int mixBlock, CutBusMode mode) => throw new NotImplementedException();

        public record struct MixBlockState(int Program, int Preview);
    }
}
