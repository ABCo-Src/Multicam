using ABCo.Multicam.Server.Features.Switchers.Core;

namespace ABCo.Multicam.Server.Features.Switchers.Buffering
{
	public interface IMixBlockInteractionEmulator
    {
        bool TrySetProgWithPreviewThenCut(int val);
        void CutWithSetProgAndPrev();
        void SetCutBusWithProgSet(int val);
        bool TrySetCutBusWithPrevThenAuto(int val);
    }

    public class MixBlockInteractionEmulator : IMixBlockInteractionEmulator
    {
        readonly SwitcherMixBlock _mixBlock;
        readonly int _mixBlockIdx;
        readonly IRawSwitcher _switcher;
        readonly IMixBlockInteractionBuffer _parent;

        public MixBlockInteractionEmulator(SwitcherMixBlock mixBlock, int mixBlockIdx, IRawSwitcher switcher, IMixBlockInteractionBuffer parent)
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
