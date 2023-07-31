using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface ICutBusInteractionBuffer : IMixBlockInteractionBuffer { }
    public class CutBusInteractionBuffer : ICutBusInteractionBuffer
    {
        ISwitcher _switcher;
        SwitcherMixBlock _mixBlock;
        int _mixBlockIdx;

        int _programValue;
        int _previewValue;

        public CutBusInteractionBuffer(ISwitcher switcher, SwitcherMixBlock mixBlock, int index)
        {
            _switcher = switcher;
            _mixBlock = mixBlock;
            _mixBlockIdx = index;

            _programValue = switcher.ReceiveValue(_mixBlockIdx, 0);
            _previewValue = mixBlock.ProgramInputs.Count == 0 ? 0 : mixBlock.ProgramInputs[0].Id;
        }

        public int GetValue(int bus) => bus == 0 ? _programValue : _previewValue;
        public void SetBusValue(int bus, int val)
        {
            if (bus == 0)
                _switcher.PostValue(_mixBlockIdx, bus, val);
            else
                _previewValue = val;
        }

        public void Cut()
        {
            // Swap program and preview and send the new bus value to the switcher
            throw new NotImplementedException();
        }
    }
}
