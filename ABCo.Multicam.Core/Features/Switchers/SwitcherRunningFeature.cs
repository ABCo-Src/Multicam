﻿using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public interface ISwitcherRunningFeature : IRunningFeature 
    { 
        SwitcherSpecs SwitcherSpecs { get; }
    }

    /// <summary>
    /// Represents an active switcher in the project. Provides caching, preview bus emulation and some event handling over the raw ISwitcher.
    /// </summary>
    public class SwitcherRunningFeature : ISwitcherRunningFeature
    {
        ISwitcher _rawSwitcher;
        MixBlockStore[] _store;

        public SwitcherSpecs SwitcherSpecs { get; private set; }
        public bool IsConnected { get; private set; }

        public SwitcherRunningFeature(IDummySwitcher switcher)
        {
            _rawSwitcher = switcher;
            SwitcherSpecs = switcher.ReceiveSpecs();
            IsConnected = true;

            // Initialize store to all 1s, a known valid state for dummy switchers
            _store = new MixBlockStore[SwitcherSpecs.MixBlocks.Count];
            Array.Fill(_store, new MixBlockStore(1, 1));
        }

        public int GetValue(int mixBlock, int bus) => bus == 0 ? _store[mixBlock].Program : _store[mixBlock].Preview;

        public async Task SetValueAndWaitAsync(int mixBlock, int bus, int value)
        {
            if (!IsConnected) return;

            // Native
            if (bus == 0 || SwitcherSpecs.MixBlocks[mixBlock].NativeType == SwitcherMixBlockType.ProgramPreview)
                await _rawSwitcher.SendValueAsync(mixBlock, bus, value);

            // Emulated
            else
                _store[mixBlock].Preview = value;
        }

        public async Task ChangeSwitcherAsync(ISwitcher switcher)
        {
            if (switcher.IsConnected)
            {
                // Update details instantly
                var specs = await switcher.ReceiveSpecsAsync();
                var newStore = new MixBlockStore[specs.MixBlocks.Count];
                await UpdateStoreValues(switcher, specs, newStore);

                SwitcherSpecs = specs;
                _store = newStore;
                IsConnected = true;
            }
            else
            {
                // Disconnected - use completely blank specs
                SwitcherSpecs = new();
                _store = Array.Empty<MixBlockStore>();
                IsConnected = false;
            }

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
