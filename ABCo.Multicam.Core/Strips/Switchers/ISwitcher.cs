using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips.Switchers
{
    public interface ISwitcher : IDisposable
    {
        SwitcherSpecs Specs { get; }

        void Connect();
        void Disconnect();
        int GetBusValue(int mixBlock, int bus);
        void SetBusValue(int mixBlock, int bus, int newValue);
    }
}