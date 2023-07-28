using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public class SwitcherRunningFeature : ISwitcherRunningFeature
    {
        // The raw underlying switcher
        //ISwitcher _rawSwitcher;

        // The buffer that adds preview emulation, caching and more to all the switcher interactions.
        // A new one is created anytime the specs change (which is why it's broken into its own service, it's an easy way to avoid async data tearing when switcher (specs) are changed).
        ISwitcherInteractionBuffer _buffer;
        ISwitcherInteractionBufferFactory _bufferFactory;

        public bool IsConnected => _buffer.IsConnected;
        public SwitcherSpecs SwitcherSpecs => _buffer.Specs;
        
        public SwitcherRunningFeature(IDummySwitcher dummySwitcher, ISwitcherInteractionBufferFactory bufferFactory)
        {
            _bufferFactory = bufferFactory;
            _buffer = bufferFactory.CreateDummy(dummySwitcher);
        }

        public int GetValue(int mixBlock, int bus) => _buffer.GetValue(mixBlock, bus);
        public Task SetValueAndWaitAsync(int mixBlock, int bus, int value) => _buffer.SetValueAsync(mixBlock, bus, value);
        public async void SetValueBackground(int mixBlock, int bus, int value) => await SetValueAndWaitAsync(mixBlock, bus, value);

        public async Task ChangeSwitcherAsync(ISwitcher switcher)
        {
            if (switcher is IDummySwitcher dummy)
                _buffer = _bufferFactory.CreateDummy(dummy);
            else
                _buffer = await _bufferFactory.CreateRealAsync(switcher);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
