using ABCo.Multicam.Core.Features.Switchers.Fading;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IMixBlockInteractionBuffer 
    {
        int Program { get; }
        int Preview { get; }
        CutBusMode CutBusMode { get; }

        void SetProgram(int val);
        void SetPreview(int val);
        void Cut();
        void Auto();
        void SetCutBus(int val);
        void SetCutBusMode(CutBusMode val);

        void SetCacheChangeExceptRefreshCall(Action<RetrospectiveFadeInfo>? onCacheChange);
        void RefreshCache();
        void RefreshWithKnownProg(int val);
        void RefreshWithKnownPrev(int val);
    }

    public class MixBlockInteractionBuffer : IMixBlockInteractionBuffer
    {
        readonly SwitcherMixBlock _mixBlock;
        readonly int _mixBlockIdx;
        readonly ISwitcher _switcher;
        readonly IMixBlockInteractionEmulator _fallbackEmulator;
        Action<RetrospectiveFadeInfo>? _onCacheChangeExceptRefresh;

        public int Program { get; private set; }
        public int Preview { get; private set; }
        public CutBusMode CutBusMode { get; private set; }

        public MixBlockInteractionBuffer(SwitcherMixBlock block, int mixBlockIdx, ISwitcher switcher, ISwitcherInteractionBufferFactory factory)
        {
            _mixBlock = block;
            _mixBlockIdx = mixBlockIdx;
            _switcher = switcher;
            _fallbackEmulator = factory.CreateMixBlockEmulator(block, mixBlockIdx, switcher, this);

            Program = switcher.ReceiveValue(mixBlockIdx, 0);

            if (block.SupportedFeatures.SupportsDirectPreviewAccess)
                Preview = switcher.ReceiveValue(mixBlockIdx, 1);
            else
                Preview = block.ProgramInputs.Count == 0 ? 0 : block.ProgramInputs[0].Id;

            CutBusMode = _mixBlock.SupportedFeatures.SupportsCutBusModeChanging ? _switcher.GetCutBusMode(_mixBlockIdx) : CutBusMode.Cut;
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

        public void Cut()
        {
            if (_mixBlock.SupportedFeatures.SupportsCutAction)
                _switcher.Cut(_mixBlockIdx);
            else
                _fallbackEmulator.CutWithSetProgAndPrev();
        }

        public void Auto() => throw new NotImplementedException();

        public void SetCutBus(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsCutBusSwitching)
            {
                _switcher.SetCutBus(_mixBlockIdx, val);
                return;
            }

            if (CutBusMode == CutBusMode.Auto && _fallbackEmulator.TrySetCutBusWithPrevThenAuto(val)) return;
            _fallbackEmulator.SetCutBusWithProgSet(val);
        }

        public void SetCutBusMode(CutBusMode val)
        {
            if (_mixBlock.SupportedFeatures.SupportsCutBusModeChanging)
                _switcher.SetCutBusMode(_mixBlockIdx, val);
            else
                CutBusMode = val;
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
