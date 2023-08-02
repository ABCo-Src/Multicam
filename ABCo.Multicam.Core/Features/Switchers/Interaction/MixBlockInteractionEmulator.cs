using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IMixBlockInteractionEmulator 
    {
        bool TrySetProgWithPreviewThenCutAction(int val);
        bool TrySetProgWithCutBusCutMode(int val);
        bool TrySetProgWithCutBusAutoMode(int val);
    }

    public class MixBlockInteractionEmulator : IMixBlockInteractionEmulator
    {
        SwitcherMixBlock _mixBlock;
        int _mixBlockIdx;
        ISwitcher _switcher;
        IMixBlockInteractionBuffer _parent;

        public MixBlockInteractionEmulator(SwitcherMixBlock mixBlock, int mixBlockIdx, IMixBlockInteractionBuffer parent, ISwitcher switcher) 
        {
            _mixBlock = mixBlock;
            _mixBlockIdx = mixBlockIdx;
            _switcher = switcher;
            _parent = parent;
        }

        public bool TrySetProgWithPreviewThenCutAction(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsDirectPreviewAccess && _mixBlock.SupportedFeatures.SupportsCutAction)
            {
                _switcher.PostValue(_mixBlockIdx, 0, val);
                _switcher.Cut(_mixBlockIdx);
                return true;
            }

            return false;
        }

        public bool TrySetProgWithCutBusCutMode(int val)
        {
            if (_mixBlock.SupportedFeatures.SupportsCutBusSwitching && _mixBlock.SupportedFeatures.SupportsCutBusCutMode)
                return UseCutBusWithMode(CutBusMode.Cut, val);

            return false;
        }

        public bool TrySetProgWithCutBusAutoMode(int val)
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
                _switcher.SetCutBusMode(mode);
            }

            // Make the cut
            _switcher.SetCutBus(_mixBlockIdx, val);
            return true;
        }
    }
}
