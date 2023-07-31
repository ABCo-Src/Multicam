using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IMixBlockInteractionBuffer 
    {
        int GetValue(int bus);
        void SetBusValue(int bus, int val);
    }
}
