using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips.Switchers
{
    /// <summary>
    /// The raw surface to talk to the switcher. 
    /// None of these operations are likely to be cached (the runner provides caching), and all interactions must be valid 
    /// (no changing the preview bus if there is none, for instance, it's the runner's job to emulate this if needed).
    /// </summary>
    public interface ISwitcher : IDisposable
    {
        /// <summary>
        /// Establishes a connection with the physical switcher
        /// </summary>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the physical switcher
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Contacts the switcher and receives its current specifications.
        /// </summary>
        Task<SwitcherSpecs> ReceiveSpecsAsync();

        /// <summary>
        /// Contacts the switcher and receives the current value (with no cache) stored in the given mix block.
        /// </summary>
        /// <param name="bus">The bus within the block. 0 is always program, and 1 may be preview IF the switcher supports it natively.</param>
        Task<int> ReceiveValueAsync(int mixBlock, int bus);

        /// <summary>
        /// Contacts the switcher and sends a new value.
        /// </summary>
        /// <param name="bus">The bus within the block. 0 is always program, and 1 may be preview IF the switcher supports it natively.</param>
        Task SendValueAsync(int mixBlock, int bus, int id);
    }
}