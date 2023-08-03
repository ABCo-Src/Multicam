﻿using ABCo.Multicam.Core.Features.Switchers.Fading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IMixBlockInteractionBuffer 
    {
        int Program { get; }
        int Preview { get; }
        CutBusMode CutBusMode { get; }

        void SetProgram(int val);
        void SetPreview(int val);
        void SetCacheChangeExceptRefreshCall(Action<RetrospectiveFadeInfo>? onCacheChange);
        void RefreshCache();
        void RefreshWithKnownProg(int val);
        void RefreshWithKnownPrev(int val);
    }

    public class MixBlockInteractionBuffer : IMixBlockInteractionBuffer
    {
        SwitcherMixBlock _mixBlock;
        int _mixBlockIdx;
        ISwitcher _switcher;
        IMixBlockInteractionEmulator _fallbackEmulator;
        Action<RetrospectiveFadeInfo>? _onCacheChangeExceptRefresh;

        public int Program { get; private set; }
        public int Preview { get; private set; }
        public CutBusMode CutBusMode { get; private set; }

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
            if (_fallbackEmulator.TrySetProgWithPreviewThenCut(val)) return;
            if (_fallbackEmulator.TrySetProgWithCutBusCut(val)) return;
            if (_fallbackEmulator.TrySetProgWithCutBusAuto(val)) return;

            // If neither works, just update the cache
            Program = val;
            _onCacheChangeExceptRefresh?.Invoke(new());
        }

        public void SetPreview(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess)
                _switcher.PostValue(_mixBlockIdx, 1, val);
            else
            {
                Preview = val;
                _onCacheChangeExceptRefresh?.Invoke(new());
            }
        }

        public void RefreshCache()
        {
            Program = _switcher.ReceiveValue(_mixBlockIdx, 0);

            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess)
                Preview = _switcher.ReceiveValue(_mixBlockIdx, 1);
        }

        public void RefreshWithKnownProg(int knownProg) => Program = knownProg;
        public void RefreshWithKnownPrev(int knownPrev) => Preview = knownPrev;
        public void SetCacheChangeExceptRefreshCall(Action<RetrospectiveFadeInfo>? cacheChange) => _onCacheChangeExceptRefresh = cacheChange;
    }
}
