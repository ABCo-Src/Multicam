using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public interface ISwitcherRunningFeature : IRunningFeature
    {
        SwitcherSpecs SwitcherSpecs { get; }
        void SetOnBusChangeForVM(Action? callback);
        int GetValue(int mixBlock, int bus);
    }

    public class SwitcherRunningFeature : ISwitcherRunningFeature
    {
        // The raw underlying switcher
        //ISwitcher _rawSwitcher;

        // TODO: Add slamming protection
        // TODO: Add error handling

        // The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
        // A new one is created anytime the specs change (which is why it's broken into its own service, it's an easy way to avoid async data tearing when switcher (specs) are changed).
        ISwitcherInteractionBuffer _buffer;
        ISwitcherInteractionBufferFactory _bufferFactory;
        Action? _busChangeCallback;

        public bool IsConnected => _buffer.IsConnected;
        public SwitcherSpecs SwitcherSpecs => _buffer.Specs;
        
        public SwitcherRunningFeature(IDummySwitcher dummySwitcher, ISwitcherInteractionBufferFactory bufferFactory)
        {
            _bufferFactory = bufferFactory;
            _buffer = bufferFactory.CreateDummy(dummySwitcher);
            _buffer.SetOnBusChangeCallback(OnBusChange);
        }

        public int GetValue(int mixBlock, int bus) => _buffer.GetValue(mixBlock, bus);
        public void PostValue(int mixBlock, int bus, int value) => _buffer.PostValue(mixBlock, bus, value);

        public async Task ChangeSwitcherAsync(ISwitcher switcher)
        {
            var oldBuffer = _buffer;

            if (switcher is IDummySwitcher dummy)
                _buffer = _bufferFactory.CreateDummy(dummy);
            else
                _buffer = await _bufferFactory.CreateRealAsync(switcher);

            _buffer.SetOnBusChangeCallback(OnBusChange);
            oldBuffer.Dispose();
        }

        void OnBusChange() => _busChangeCallback?.Invoke();
        public void SetOnBusChangeForVM(Action? callback) => _busChangeCallback = callback;

        public void Dispose() => _buffer.Dispose();
    }
}
