using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public class DummySwitcherInteractionBuffer : ISwitcherInteractionBuffer
    {
        IDummySwitcher _switcher;
        public DummySwitcherInteractionBuffer(IDummySwitcher switcher) => _switcher = switcher;

        public bool IsConnected => true;
        public SwitcherSpecs Specs => _switcher.ReceiveSpecs();
        public int GetValue(int mixBlock, int bus) => _switcher.ReceiveValue(mixBlock, bus);
        public void PostValue(int mixBlock, int bus, int value) => _switcher.PostValue(mixBlock, bus, value);
        public void Dispose() => _switcher.Dispose();
    }
}
