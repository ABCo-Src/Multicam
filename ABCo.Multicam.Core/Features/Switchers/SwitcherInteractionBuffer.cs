using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public interface ISwitcherInteractionBuffer : IDisposable
    {
        bool IsConnected { get; }
        SwitcherSpecs Specs { get; }
        int GetValue(int mixBlock, int bus);
        void PostValue(int mixBlock, int bus, int value);
        void SetOnBusChangeCallback(Action? callback);
    }

    public class SwitcherInteractionBuffer : ISwitcherInteractionBuffer
    {
        readonly ISwitcher _rawSwitcher;
        readonly MixBlockStore[] _store;
        Action? _onBusChangeCallback;

        public bool IsConnected { get; private set; }
        public SwitcherSpecs Specs { get; private set; }

        private SwitcherInteractionBuffer(ISwitcher switcher, bool isConnected, MixBlockStore[] store, SwitcherSpecs specs) =>
            (_rawSwitcher, IsConnected, _store, Specs) = (switcher, isConnected, store, specs);

        public static Task<SwitcherInteractionBuffer> CreateAsync(ISwitcher switcher) => Task.Run(() => Create(switcher));
        public static SwitcherInteractionBuffer Create(ISwitcher switcher)
        {
            if (switcher.IsConnected)
            {
                var specs = switcher.ReceiveSpecs();
                var newBuffer = new SwitcherInteractionBuffer(switcher, true, CreateStore(switcher, specs), specs);
                switcher.SetOnBusChangeCallback(newBuffer.OnBusChange);
                return newBuffer;
            }
            else
                return new SwitcherInteractionBuffer(switcher, false, Array.Empty<MixBlockStore>(), new());
        }

        static MixBlockStore[] CreateStore(ISwitcher switcher, SwitcherSpecs specs)
        {
            var newStore = new MixBlockStore[specs.MixBlocks.Count];
            for (int i = 0; i < newStore.Length; i++)
            {
                var mixBlock = specs.MixBlocks[i];

                // Program
                newStore[i].Program = switcher.ReceiveValue(i, 0);

                // Preview
                if (mixBlock.NativeType == SwitcherMixBlockType.CutBus)
                    newStore[i].Preview = mixBlock.ProgramInputs.Count == 0 ? 0 : mixBlock.ProgramInputs[0].Id;
                else
                    newStore[i].Preview = switcher.ReceiveValue(i, 1);
            }
            return newStore;
        }

        public void Dispose() => _rawSwitcher.Dispose();

        public int GetValue(int mixBlock, int bus) => bus == 0 ? _store[mixBlock].Program : _store[mixBlock].Preview;
        public void PostValue(int mixBlock, int bus, int value)
        {
            if (!IsConnected) return;

            // Native
            if (bus == 0 || Specs.MixBlocks[mixBlock].NativeType == SwitcherMixBlockType.ProgramPreview)
                _rawSwitcher.PostValue(mixBlock, bus, value);

            // Emulated
            else
                _store[mixBlock].Preview = value;
        }

        void OnBusChange(SwitcherBusChangeInfo info)
        {
            // If the bus is known, update directly
            if (info.IsBusKnown)
            {
                if (info.Bus == 0)
                    _store[info.MixBlock].Program = info.NewValue;
                else
                    _store[info.MixBlock].Preview = info.NewValue;
            }

            // Otherwise, re-read every bus
            else
            {
                for (int i = 0; i < _store.Length; i++)
                {
                    _store[i].Program = _rawSwitcher.ReceiveValue(i, 0);
                    
                    // Only modify preview if not emulated
                    if (Specs.MixBlocks[i].NativeType == SwitcherMixBlockType.ProgramPreview)
                        _store[i].Preview = _rawSwitcher.ReceiveValue(i, 1);
                }
            }

            _onBusChangeCallback?.Invoke();
        }

        public void SetOnBusChangeCallback(Action? callback) => _onBusChangeCallback = callback;

        record struct MixBlockStore(int Program, int Preview);
    }

    public interface ISwitcherInteractionBufferFactory
    {
        Task<ISwitcherInteractionBuffer> CreateRealAsync(ISwitcher switcher);
        ISwitcherInteractionBuffer CreateDummy(IDummySwitcher switcher);
    }

    public class SwitcherInteractionBufferFactory : ISwitcherInteractionBufferFactory
    {
        public ISwitcherInteractionBuffer CreateDummy(IDummySwitcher switcher) => new DummySwitcherInteractionBuffer(switcher);
        public async Task<ISwitcherInteractionBuffer> CreateRealAsync(ISwitcher switcher) => await SwitcherInteractionBuffer.CreateAsync(switcher);
    }
}
