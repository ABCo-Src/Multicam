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
        Task SetValueAsync(int mixBlock, int bus, int value);
    }

    public class SwitcherInteractionBuffer : ISwitcherInteractionBuffer
    {
        readonly ISwitcher _rawSwitcher;
        readonly MixBlockStore[] _store;

        public bool IsConnected { get; private set; }
        public SwitcherSpecs Specs { get; private set; }

        private SwitcherInteractionBuffer(ISwitcher switcher, bool isConnected, MixBlockStore[] store, SwitcherSpecs specs) =>
            (_rawSwitcher, IsConnected, _store, Specs) = (switcher, isConnected, store, specs);

        public static async Task<SwitcherInteractionBuffer> Create(ISwitcher switcher)
        {
            if (switcher.IsConnected)
            {
                var specs = await switcher.ReceiveSpecsAsync();
                return new SwitcherInteractionBuffer(switcher, true, await CreateStore(switcher, specs), specs);
            }
            else 
                return new SwitcherInteractionBuffer(switcher, false, Array.Empty<MixBlockStore>(), new());
        }

        static async Task<MixBlockStore[]> CreateStore(ISwitcher switcher, SwitcherSpecs specs)
        {
            var newStore = new MixBlockStore[specs.MixBlocks.Count];
            for (int i = 0; i < newStore.Length; i++)
            {
                var mixBlock = specs.MixBlocks[i];

                // Program
                newStore[i].Program = await switcher.ReceiveValueAsync(i, 0);

                // Preview
                if (mixBlock.NativeType == SwitcherMixBlockType.CutBus)
                    newStore[i].Preview = mixBlock.ProgramInputs.Count == 0 ? 0 : mixBlock.ProgramInputs[0].Id;
                else
                    newStore[i].Preview = await switcher.ReceiveValueAsync(i, 1);
            }
            return newStore;
        }

        public void Dispose() => _rawSwitcher.Dispose();

        public int GetValue(int mixBlock, int bus) => bus == 0 ? _store[mixBlock].Program : _store[mixBlock].Preview;
        public async Task SetValueAsync(int mixBlock, int bus, int value)
        {
            if (!IsConnected) return;

            // Native
            if (bus == 0 || Specs.MixBlocks[mixBlock].NativeType == SwitcherMixBlockType.ProgramPreview)
                await _rawSwitcher.SendValueAsync(mixBlock, bus, value);

            // Emulated
            else
                _store[mixBlock].Preview = value;
        }

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
        public async Task<ISwitcherInteractionBuffer> CreateRealAsync(ISwitcher switcher) => await SwitcherInteractionBuffer.Create(switcher);
    }
}
