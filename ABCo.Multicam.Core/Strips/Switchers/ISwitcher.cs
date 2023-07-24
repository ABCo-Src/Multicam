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

        /// <summary>
        /// Establishes a connection with the physical switcher
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the physical switcher
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Retrieves the current value (with no cache) stored in the switcher.
        /// </summary>
        /// <param name="bus">The bus within the block. 0 is always program, and 1 may be preview IF the switcher supports it natively.</param>
        int ReceiveValue(int mixBlock, int bus);

        /// <summary>
        /// Sets the current value (with no cache) stored in the switcher.
        /// </summary>
        /// <param name="bus">The bus within the block. 0 is always program, and 1 may be preview IF the switcher supports it natively.</param>
        void SetValue(int mixBlock, int bus, int id);
    }
}