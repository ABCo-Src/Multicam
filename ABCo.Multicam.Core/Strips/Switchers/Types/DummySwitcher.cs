using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Core.Strips.Switchers;

namespace ABCo.Multicam.Core.Strips.Switchers.Types
{
    public interface IDummySwitcher : ISwitcher 
    {
        // Non-async variant so specs can be immediately received, allows the runner to neatly use a dummy switcher as an initial safe state.
        SwitcherSpecs ReceiveSpecs();
        void UpdateSpecs(DummyMixBlock[] mixBlocks);
    }

    public class DummySwitcher : IDummySwitcher
    {
        SwitcherSpecs _specs;
        int _program = 1;
        int _preview = 1;

        public DummySwitcher() 
        {
            var mixBlk1Inputs = new SwitcherBusInput[]
            {
                new SwitcherBusInput(1, "Cam 1"),
                new SwitcherBusInput(2, "Cam 2"),
                new SwitcherBusInput(3, "Cam 3"),
                new SwitcherBusInput(4, "Cam 4")
            };

            _specs = new SwitcherSpecs(
                new SwitcherMixBlock[]
                {
                    new SwitcherMixBlock(SwitcherMixBlockInputType.ProgramPreview, mixBlk1Inputs, mixBlk1Inputs)
                }
            );
        }

        public SwitcherSpecs ReceiveSpecs() => _specs;
        public Task<SwitcherSpecs> ReceiveSpecsAsync() => Task.FromResult(ReceiveSpecs());

        public void UpdateSpecs(DummyMixBlock[] mixBlocks) 
        {
            var mixBlocksArray = new SwitcherMixBlock[mixBlocks.Length];

            for (int i = 0; i < mixBlocks.Length; i++)
            {
                // Create the program array
                var programArray = new SwitcherBusInput[mixBlocks[i].InputCount];
                for (int j = 0; j < programArray.Length; j++)
                    programArray[j] = new SwitcherBusInput(j + 1, "Cam " + (j + 1));

                // Determine the relevant preview array and create the final mix block
                var previewArray = mixBlocks[i].Type == SwitcherMixBlockInputType.ProgramPreview ? programArray : null;                
                mixBlocksArray[i] = new SwitcherMixBlock(mixBlocks[i].Type, programArray, previewArray);
            }

            _specs = new SwitcherSpecs(mixBlocksArray);            
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> ReceiveValueAsync(int mixBlock, int bus)
        {
            ValidateMixBlockAndBus(mixBlock, bus);
            return Task.FromResult(bus == 0 ? _program : _preview);
        }

        private void ValidateMixBlockAndBus(int mixBlock, int bus)
        {
            // Validate mix block
            if (mixBlock < 0 || mixBlock >= _specs.MixBlocks.Count) throw new ArgumentException("Invalid mix block given to DummySwitcher");

            // Validate bus
            if (bus == 0) return;
            if (bus == 1 && _specs.MixBlocks[mixBlock].NativeType == SwitcherMixBlockInputType.ProgramPreview) return;

            throw new ArgumentException("Invalid bus given to DummySwitcher");
        }

        public Task SendValueAsync(int mixBlock, int bus, int newValue)
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
                _program = newValue;
            else
                _preview = newValue;

            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    public struct DummyMixBlock
    {
        public int InputCount;
        public SwitcherMixBlockInputType Type;

        public DummyMixBlock(int inputCount, SwitcherMixBlockInputType type) => (InputCount, Type) = (inputCount, type);
    }

}
