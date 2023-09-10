using ABCo.Multicam.Core.Features.Switchers.Data.Config;

namespace ABCo.Multicam.Core.Features.Switchers.Types
{
	public interface IDummySwitcher : ISwitcher, IParameteredService<DummySwitcherConfig> { }

    public class DummySwitcher : Switcher, IDummySwitcher
    {
		readonly SwitcherSpecs _specs = null!;
		readonly MixBlockState[] _states = null!;

        public DummySwitcher(DummySwitcherConfig config)
        {
            _specs = CreateSpecsFrom(config.MixBlocks);

            // Create new state, starting at 1
            _states = new MixBlockState[_specs.MixBlocks.Count];
            Array.Fill(_states, new MixBlockState(1, 1));
        }

		public override void RefreshSpecs() => _eventHandler?.OnSpecsChange(_specs);

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

        public override void RefreshConnectionStatus() => _eventHandler?.OnConnectionStateChange(true);

        public override void RefreshProgram(int mixBlock)
        {
            ValidateMixBlock(mixBlock);
            _eventHandler?.OnProgramValueChange(new SwitcherProgramChangeInfo(mixBlock, _states[mixBlock].Program, null));
        }

        public override void RefreshPreview(int mixBlock)
        {
            ValidateMixBlock(mixBlock);
            _eventHandler?.OnPreviewValueChange(new SwitcherPreviewChangeInfo(mixBlock, _states[mixBlock].Preview, null));
        }

        public override void SendProgramValue(int mixBlock, int newValue)
        {
            ValidateMixBlock(mixBlock);
            ValidateInput(mixBlock, newValue);

            _states[mixBlock].Program = newValue;
            _eventHandler?.OnProgramValueChange(new SwitcherProgramChangeInfo(mixBlock, _states[mixBlock].Program, null));
        }

        void ValidateInput(int mixBlock, int newValue)
        {
            for (int i = 0; i < _specs.MixBlocks[mixBlock].ProgramInputs.Count; i++)
                if (_specs.MixBlocks[mixBlock].ProgramInputs[i].Id == newValue)
                    return;

            throw new ArgumentException("Invalid input ID given to DummySwitcher");
        }

        public override void SendPreviewValue(int mixBlock, int newValue)
        {
            ValidateMixBlock(mixBlock);
            ValidateInput(mixBlock, newValue);

            _states[mixBlock].Preview = newValue;
            _eventHandler?.OnPreviewValueChange(new SwitcherPreviewChangeInfo(mixBlock, _states[mixBlock].Preview, null));
        }

        void ValidateMixBlock(int mixBlock)
        {
            if (mixBlock < 0 || mixBlock >= _specs.MixBlocks.Count) 
                throw new ArgumentException("Invalid mix block given to DummySwitcher");
        }

        public override void Dispose() { }

        public record struct MixBlockState(int Program, int Preview);
    }
}