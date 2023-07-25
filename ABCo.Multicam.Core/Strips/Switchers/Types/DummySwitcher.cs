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
            return Task.FromResult(bus == 0 ? _program : _preview);
        }

        public Task SendValueAsync(int mixBlock, int bus, int newValue)
        {
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
