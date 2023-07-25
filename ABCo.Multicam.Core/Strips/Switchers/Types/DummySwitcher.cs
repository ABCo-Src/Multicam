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
        // Non-async variant so specs can be immediately received for the runner to have *something* to start with initially.
        SwitcherSpecs ReceiveSpecs();
    }
    public class DummySwitcher : IDummySwitcher
    {
        SwitcherSpecs _specs;

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
                    new SwitcherMixBlock(SwitcherBusInputType.ProgramPreview, mixBlk1Inputs, mixBlk1Inputs)
                }
            );
        }

        public SwitcherSpecs ReceiveSpecs() => _specs;
        public Task<SwitcherSpecs> ReceiveSpecsAsync() => Task.FromResult(ReceiveSpecs());

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<int> ReceiveValueAsync(int mixBlock, int bus)
        {
            throw new NotImplementedException();
        }

        public Task SendValueAsync(int mixBlock, int bus, int newValue)
        {
            throw new NotImplementedException();
        }
    }
}
