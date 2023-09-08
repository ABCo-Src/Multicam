namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
	public interface IMixBlockInteractionEmulator 
    {
        bool TrySetProgWithPreviewThenCut(int val);
        bool TrySetProgWithCutBusCut(int val);
        bool TrySetProgWithCutBusAuto(int val);
        void CutWithSetProgAndPrev();
        void SetCutBusWithProgSet(int val);
        bool TrySetCutBusWithPrevThenAuto(int val);
    }

    public class MixBlockInteractionEmulator : IMixBlockInteractionEmulator
    {
        readonly SwitcherMixBlock _mixBlock;
        readonly int _mixBlockIdx;
        readonly ISwitcher _switcher;
        readonly IMixBlockInteractionBuffer _parent;

        public MixBlockInteractionEmulator(SwitcherMixBlock mixBlock, int mixBlockIdx, ISwitcher switcher, IMixBlockInteractionBuffer parent) 
        {
            _mixBlock = mixBlock;
            _mixBlockIdx = mixBlockIdx;
            _switcher = switcher;
            _parent = parent;
        }

        public bool TrySetProgWithPreviewThenCut(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess && _mixBlock.SupportedFeatures.SupportsCutAction)
            {
                _switcher.SendPreviewValue(_mixBlockIdx, val);
                _switcher.Cut(_mixBlockIdx);
                return true;
            }

            return false;
        }

        public bool TrySetProgWithCutBusCut(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsCutBusSwitching && _mixBlock.SupportedFeatures.SupportsCutBusCutMode)
                return UseCutBusWithMode(CutBusMode.Cut, val);

            return false;
        }

        public bool TrySetProgWithCutBusAuto(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsCutBusSwitching && _mixBlock.SupportedFeatures.SupportsCutBusAutoMode)            
                return UseCutBusWithMode(CutBusMode.Auto, val);

            return false;
        }

        bool UseCutBusWithMode(CutBusMode mode, int val)
        {
            // If we need to switch modes, verify that we can, and do it if so.
            if (_parent.CutBusMode != mode)
            {
                if (!_mixBlock.SupportedFeatures.SupportsCutBusModeChanging) return false;
                _switcher.SetCutBusMode(_mixBlockIdx, mode);
            }

            // Make the cut
            _switcher.SetCutBus(_mixBlockIdx, val);
            return true;
        }

        public void CutWithSetProgAndPrev()
        {
            int oldPrev = _parent.Preview;
            int oldProg = _parent.Program;
            _parent.SendPreview(oldProg);
            _parent.SendProgram(oldPrev);
        }

        public void SetCutBusWithProgSet(int val) => _parent.SendProgram(val);

        public bool TrySetCutBusWithPrevThenAuto(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess && _mixBlock.SupportedFeatures.SupportsAutoAction)
            {
                _parent.SendPreview(val);
                _parent.Auto();
                return true;
            }

            return false;
        }
    }
}
