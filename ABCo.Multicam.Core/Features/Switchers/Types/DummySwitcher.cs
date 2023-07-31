using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Core.Features.Switchers;

namespace ABCo.Multicam.Core.Features.Switchers.Types
{
    public interface IDummySwitcher : ISwitcher
    {
        void UpdateSpecs(DummyMixBlock[] mixBlocks);
    }

    public class DummySwitcher : IDummySwitcher
    {
        SwitcherSpecs _specs;
        MixBlockState[] _states;
        Action<SwitcherBusChangeInfo>? _busChangeFinishCallback;

        public DummySwitcher()
        {
            (_specs, _states) = (null!, null!); // Assigned by UpdateSpecs
            UpdateSpecs(new DummyMixBlock[] { new(4, SwitcherMixBlockType.ProgramPreview), new(4, SwitcherMixBlockType.ProgramPreview) });            
        }

        public SwitcherSpecs ReceiveSpecs() => _specs;

        public void UpdateSpecs(params DummyMixBlock[] mixBlocks)
        {
            _specs = CreateSpecsFrom(mixBlocks);

            // Create new state, starting at 1
            _states = new MixBlockState[_specs.MixBlocks.Count];
            Array.Fill(_states, new MixBlockState(1, 1));
        }

        public static SwitcherSpecs CreateSpecsFrom(params DummyMixBlock[] mixBlocks)
        {
            var mixBlocksArray = new SwitcherMixBlock[mixBlocks.Length];

            for (int i = 0; i < mixBlocks.Length; i++)
            {
                // Create the program array
                var programArray = new SwitcherBusInput[mixBlocks[i].InputCount];
                for (int j = 0; j < programArray.Length; j++)
                    programArray[j] = new SwitcherBusInput(j + 1, "Cam " + (j + 1));

                // Create the mix block
                if (mixBlocks[i].Type == SwitcherMixBlockType.ProgramPreview)
                    mixBlocksArray[i] = SwitcherMixBlock.NewProgPrevSameInputs(programArray);
                else
                    mixBlocksArray[i] = SwitcherMixBlock.NewCutBus(programArray);
            }

            return new SwitcherSpecs(mixBlocksArray);
        }

        public bool IsConnected => true;
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

        public void Cut(int mixBlockIdx)
        {
            // Validate mix block
            if (mixBlockIdx < 0 || mixBlockIdx >= _specs.MixBlocks.Count) throw new ArgumentException("Invalid mix block given to DummySwitcher");

            if (_specs.MixBlocks[mixBlockIdx].NativeType == SwitcherMixBlockType.CutBus) throw new NotSupportedException();

            var mixBlockState = _states[mixBlockIdx];
            (mixBlockState.Program, mixBlockState.Preview) = (mixBlockState.Preview, mixBlockState.Program);
            _states[mixBlockIdx] = mixBlockState;

            _busChangeFinishCallback?.Invoke(new(true, mixBlockIdx, 0, mixBlockState.Program, null));
            _busChangeFinishCallback?.Invoke(new(true, mixBlockIdx, 1, mixBlockState.Preview, null));
        }

        public void Dispose() { }

        public static DummySwitcher ForSpecs(params DummyMixBlock[] mixBlocks)
        {
            var dummy = new DummySwitcher();
            dummy.UpdateSpecs(mixBlocks);
            return dummy;
        }

        public void SetOnBusChangeFinishCall(Action<SwitcherBusChangeInfo>? callback) => _busChangeFinishCallback = callback;

        struct MixBlockState
        {
            public int Program;
            public int Preview;

            public MixBlockState(int program, int preview) => (Program, Preview) = (program, preview);
        }
    }

    public struct DummyMixBlock
    {
        public int InputCount;
        public SwitcherMixBlockType Type;

        public DummyMixBlock(int inputCount, SwitcherMixBlockType type) => (InputCount, Type) = (inputCount, type);
    }
}
