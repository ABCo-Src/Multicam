using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips.Switchers
{
    public interface ISwitcherRunningStrip : IRunningStrip 
    { 
        SwitcherSpecs SwitcherSpecs { get; }
    }

    /// <summary>
    /// Represents an active switcher in the project. Provides caching, preview bus emulation and some event handling over the raw ISwitcher.
    /// </summary>
    public class SwitcherRunningStrip : ISwitcherRunningStrip
    {
        ISwitcher _rawSwitcher;
        MixBlockStore[] _store;

        public SwitcherSpecs SwitcherSpecs { get; private set; }

        public SwitcherRunningStrip(IDummySwitcher switcher)
        {
            _rawSwitcher = switcher;
            SwitcherSpecs = switcher.ReceiveSpecs();

            // Initialize store to all 1s, a known valid state for dummy switchers
            _store = new MixBlockStore[SwitcherSpecs.MixBlocks.Count];
            Array.Fill(_store, new MixBlockStore(1, 1));
        }

        public int GetValue(int mixBlock, int bus) => bus == 0 ? _store[mixBlock].Program : _store[mixBlock].Preview;

        public async Task SetValueAndWaitAsync(int mixBlock, int bus, int value)
        {
            // Native
            if (bus == 0 || SwitcherSpecs.MixBlocks[mixBlock].NativeType == SwitcherMixBlockType.ProgramPreview)
                await _rawSwitcher.SendValueAsync(mixBlock, bus, value);

            // Emulated
            else
                _store[mixBlock].Preview = value;
        }

        public async Task ChangeSwitcherAsync(ISwitcher switcher)
        {
            var specs = await switcher.ReceiveSpecsAsync();
            var newStore = new MixBlockStore[specs.MixBlocks.Count];
            await UpdateStoreValues(switcher, specs, newStore);

            SwitcherSpecs = specs;
            _store = newStore;
            _rawSwitcher.Dispose();
            _rawSwitcher = switcher;
        }

        async Task UpdateStoreValues(ISwitcher switcher, SwitcherSpecs newSpecs, MixBlockStore[] store)
        {
            for (int i = 0; i < store.Length; i++)
            {
                var mixBlock = newSpecs.MixBlocks[i];

                // Program
                store[i].Program = await switcher.ReceiveValueAsync(i, 0);

                // Preview
                if (mixBlock.NativeType == SwitcherMixBlockType.CutBus)
                    store[i].Preview = mixBlock.ProgramInputs.Count == 0 ? 0 : mixBlock.ProgramInputs[0].Id;
                else
                    store[i].Preview = await switcher.ReceiveValueAsync(i, 1);
            }
        }

        public async void SetValueBackground(int mixBlock, int bus, int value)
        {
            // TODO: Error handling
            await SetValueAndWaitAsync(mixBlock, bus, value);
        }

        public ISwitcher GetRawSwitcher() => _rawSwitcher;

        public void Dispose() => _rawSwitcher.Dispose();

        record struct MixBlockStore(int Program, int Preview);
    }
}
