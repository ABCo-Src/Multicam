using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IMixBlockInteractionBuffer 
    {
        void SetProgram(int val);
        void SetPreview(int val);
        void SetCacheChangeCall(Action<int> onCacheChange);
    }

    public class MixBlockInteractionBuffer : IMixBlockInteractionBuffer
    {
        SwitcherMixBlock _mixBlock;
        int _mixBlockIdx;
        ISwitcher _switcher;
        IMixBlockInteractionEmulator _fallbackEmulator;
        Action<int>? _onCacheChange;

        public int Program { get; set; }
        public int Preview { get; set; }

        public MixBlockInteractionBuffer(SwitcherMixBlock block, int mixBlockIdx, ISwitcher switcher, IMixBlockInteractionEmulator fallbackEmulator)
        {
            _mixBlock = block;
            _mixBlockIdx = mixBlockIdx;
            _switcher = switcher;
            _fallbackEmulator = fallbackEmulator;

            Program = switcher.ReceiveValue(mixBlockIdx, 0);

            if (block.SupportedFeatures.SupportsDirectPreviewAccess)
                Preview = switcher.ReceiveValue(mixBlockIdx, 1);
            else
                Preview = block.ProgramInputs.Count == 0 ? 0 : block.ProgramInputs[0].Id;
        }

        public void SetProgram(int val)
        {
            // Try to do it natively
            if (_mixBlock.SupportedFeatures.SupportsDirectProgramModification)
            {
                _switcher.PostValue(_mixBlockIdx, 0, val);
                return;
            }

            // Otherwise, try to use a fallback method
            if (_fallbackEmulator.TrySetProgWithPreviewThenCutAction(val)) return;
            if (_fallbackEmulator.TrySetProgWithCutBusCutMode(val)) return;
            if (_fallbackEmulator.TrySetProgWithCutBusAutoMode(val)) return;

            // If neither works, just update the cache
            Program = val;
            _onCacheChange?.Invoke(_mixBlockIdx);
        }

        public void SetPreview(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess)
                _switcher.PostValue(_mixBlockIdx, 1, val);
            else
            {
                Preview = val;
                _onCacheChange?.Invoke(_mixBlockIdx);
            }
        }

        public void RefreshCache()
        {
            Program = _switcher.ReceiveValue(_mixBlockIdx, 0);

            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess)
                Preview = _switcher.ReceiveValue(_mixBlockIdx, 1);

            _onCacheChange?.Invoke(_mixBlockIdx);
        }

        public void RefreshWithKnownProg(int knownProg)
        {
            Program = knownProg;
            _onCacheChange?.Invoke(_mixBlockIdx);
        }

        public void RefreshWithKnownPrev(int knownPrev)
        {
            Preview = knownPrev;
            _onCacheChange?.Invoke(_mixBlockIdx);
        }

        public void SetCacheChangeCall(Action<int>? cacheChange) => _onCacheChange = cacheChange;
    }
}
