using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    //public interface IProgPrevInteractionBuffer : IMixBlockInteractionBuffer { }
    //public class ProgPrevInteractionBuffer : IProgPrevInteractionBuffer
    //{
    //    ISwitcher _switcher;
    //    SwitcherMixBlock _mixBlock;
    //    int _mixBlockIdx;

    //    int _programValue;
    //    int _previewValue;

    //    public ProgPrevInteractionBuffer(ISwitcher switcher, SwitcherMixBlock mixBlock, int index)
    //    {
    //        _switcher = switcher;
    //        _mixBlock = mixBlock;
    //        _mixBlockIdx = index;

    //        _programValue = switcher.ReceiveValue(index, 0);
    //        _previewValue = switcher.ReceiveValue(index, 1);
    //    }

    //    public int GetValue(int bus) => bus == 0 ? _programValue : _previewValue;
    //    public void SetBusValue(int bus, int val) => _switcher.PostValue(_mixBlockIdx, bus, val);
    //    public void Cut() => _switcher.Cut(_mixBlockIdx);

    //    public void Refresh()
    //    {
    //        _programValue = _switcher.ReceiveValue(_mixBlockIdx, 0);
    //        _previewValue = _switcher.ReceiveValue(_mixBlockIdx, 1);
    //    }

    //    public void RefreshKnown(int bus, int newVal)
    //    {
    //        if (bus == 0)
    //            _programValue = newVal;
    //        else
    //            _previewValue = newVal;
    //    }
    //}
}
