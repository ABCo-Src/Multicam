using ABCo.Multicam.Server.Features.Switchers.Core;

namespace ABCo.Multicam.Server.Features.Switchers.Buffering
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
        readonly IRawSwitcher _switcher;
        readonly IMixBlockInteractionEmulator _fallbackEmulator;
        ISwitcherEventHandler? _eventHandler;

        public int Program { get; private set; }
        public int Preview { get; private set; }
        public CutBusMode CutBusMode { get; private set; }

        public MixBlockInteractionBuffer(SwitcherMixBlock block, int mixBlockIdx, IRawSwitcher switcher, ISwitcherInteractionBufferFactory factory)
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

            CutBusMode = CutBusMode.Cut;
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
            if (_fallbackEmulator.TrySetProgWithPreviewThenCut(Preview, val)) return;

            // If neither works, just update the cache
            Program = val;
            _eventHandler?.OnProgramValueChange(new(_mixBlockIdx, val, null));
        }

        public void SendPreview(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess)
                _switcher.SendPreviewValue(_mixBlockIdx, val);
            else
            {
                Preview = val;
                _eventHandler?.OnPreviewValueChange(new(_mixBlockIdx, val, null));
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
            if (CutBusMode == CutBusMode.Auto && _fallbackEmulator.TrySetCutBusWithPrevThenAuto(val)) return;
            _fallbackEmulator.SetCutBusWithProgSet(val);
        }

        public void SetCutBusMode(CutBusMode val)
        {
            CutBusMode = val;
        }

        public void UpdateProg(int knownProg) => Program = knownProg;
        public void UpdatePrev(int knownPrev) => Preview = knownPrev;
    }
}
