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
        public MixBlockInteractionEmulator(IMixBlockInteractionBuffer parent, ISwitcher switcher) { }

        public bool TrySetProgWithCutBusAutoMode(int val)
        {
            throw new NotImplementedException();
        }

        public bool TrySetProgWithCutBusCutMode(int val)
        {
            throw new NotImplementedException();
        }

        public bool TrySetProgWithPreviewThenCutAction(int val)
        {
            throw new NotImplementedException();
        }
    }
}
