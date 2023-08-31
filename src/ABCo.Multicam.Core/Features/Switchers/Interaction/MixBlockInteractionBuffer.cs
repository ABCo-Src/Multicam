using ABCo.Multicam.Core.Features.Switchers.Fading;
using System;
using System.Diagnostics;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IMixBlockInteractionBuffer 
    {
        int Program { get; }
        int Preview { get; }
        CutBusMode CutBusMode { get; }

        void SetEventHandler(ISwitcherEventHandler? eventHandler);

        void SendProgram(int val);
        void SendPreview(int val);
        void Cut();
        void Auto();
        void SetCutBus(int val);
        void SetCutBusMode(CutBusMode val);
        void RefreshValues();

        void UpdateProg(int val);
        void UpdatePrev(int val);
    }

    public class MixBlockInteractionBuffer : IMixBlockInteractionBuffer
    {
        readonly SwitcherMixBlock _mixBlock;
        readonly int _mixBlockIdx;
        readonly ISwitcher _switcher;
        readonly IMixBlockInteractionEmulator _fallbackEmulator;
        ISwitcherEventHandler? _eventHandler;

        public int Program { get; private set; }
        public int Preview { get; private set; }
        public CutBusMode CutBusMode { get; private set; }

        public MixBlockInteractionBuffer(SwitcherMixBlock block, int mixBlockIdx, ISwitcher switcher, ISwitcherInteractionBufferFactory factory)
        {
            _mixBlock = block;
            _mixBlockIdx = mixBlockIdx;
            _switcher = switcher;
            _fallbackEmulator = factory.CreateMixBlockEmulator(block, mixBlockIdx, switcher, this);
        }

        public void SetEventHandler(ISwitcherEventHandler? eventHandler) => _eventHandler = eventHandler;

		public void RefreshValues()
        {
            _switcher.RefreshProgram(_mixBlockIdx);

            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess)
                _switcher.RefreshPreview(_mixBlockIdx);
            else
                Preview = _mixBlock.ProgramInputs.Count == 0 ? 0 : _mixBlock.ProgramInputs[0].Id;

            CutBusMode = _mixBlock.SupportedFeatures.SupportsCutBusModeChanging ? _switcher.GetCutBusMode(_mixBlockIdx) : CutBusMode.Cut;
        }

        public void SendProgram(int val)
        {
            // Try to do it natively
            if (_mixBlock.SupportedFeatures.SupportsDirectProgramModification)
            {
                _switcher.SendProgramValue(_mixBlockIdx, val);
                return;
            }

            // Otherwise, try to use a fallback method
            if (_fallbackEmulator.TrySetProgWithPreviewThenCut(val)) return;
            if (_fallbackEmulator.TrySetProgWithCutBusCut(val)) return;
            if (_fallbackEmulator.TrySetProgWithCutBusAuto(val)) return;

            // If neither works, just update the cache
            Program = val;
            _eventHandler?.OnProgramChangeFinish(new(_mixBlockIdx, 0, val, null));
        }

        public void SendPreview(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess)
                _switcher.SendPreviewValue(_mixBlockIdx, val);
            else
            {
                Preview = val;
                _eventHandler?.OnPreviewChangeFinish(new(_mixBlockIdx, val, null));
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

        public void UpdateProg(int knownProg) => Program = knownProg;
        public void UpdatePrev(int knownPrev) => Preview = knownPrev;
    }
}
