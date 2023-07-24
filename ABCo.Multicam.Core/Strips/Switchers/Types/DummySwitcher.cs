using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Core.Strips.Switchers;

namespace ABCo.Multicam.Core.Strips.Switchers.Types
{
    public interface IDummySwitcher : ISwitcher { }
    public class DummySwitcher : IDummySwitcher
    {
        SwitcherSpecs _specs;

        public DummySwitcher() 
        {
            var mixBlk1Inputs = new SwitcherBusInput[]
            {
                new SwitcherBusInput(1, ""),
                new SwitcherBusInput(2, ""),
                new SwitcherBusInput(3, ""),
                new SwitcherBusInput(4, "")
            };

            _specs = new SwitcherSpecs(
                new SwitcherMixBlock[]
                {
                    new SwitcherMixBlock(SwitcherBusInputType.ProgramPreview, mixBlk1Inputs, mixBlk1Inputs)
                }
            );
        }

        public SwitcherSpecs ReceiveSpecs() => _specs;

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int ReceiveValue(int mixBlock, int bus)
        {
            throw new NotImplementedException();
        }

        public void SendValue(int mixBlock, int bus, int newValue)
        {
            throw new NotImplementedException();
        }
    }
}
