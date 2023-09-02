using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Types.ATEM
{
    public interface IATEMRawAPI
    {
        IBMDSwitcher Connect(string address);
        IBMDSwitcherMixEffectBlockIterator CreateMixBlockIterator(IBMDSwitcher switcher);
        IBMDSwitcherInputIterator CreateInputIterator(IBMDSwitcher switcher);
        bool MoveNext(IBMDSwitcherMixEffectBlockIterator iter, out IBMDSwitcherMixEffectBlock item);
        bool MoveNext(IBMDSwitcherInputIterator iter, out IBMDSwitcherInput item);
        long GetID(IBMDSwitcherInput input);
        string GetShortName(IBMDSwitcherInput input);
        _BMDSwitcherInputAvailability GetAvailability(IBMDSwitcherInput input);
        void Free(object obj);
    }

}
