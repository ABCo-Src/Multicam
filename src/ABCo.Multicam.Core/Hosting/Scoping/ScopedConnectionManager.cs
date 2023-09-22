using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Hosting.Scoping
{
    public interface IScopedConnectionManager
    {
        IScopeInfo CreateScope();
        void DestroyScope(IScopeInfo info);

        event Action<IScopeInfo> ScopeDestroyed;
    }

    /// <summary>
    /// The current "scope", there's one per client connection (and one for the desktop app and its interactions).
    /// Mainly used so each client can get its own "presenter" objects assigned.
    /// </summary>
    public interface IScopeInfo 
    {
        int ConnectionID { get; }
    }

    public class ScopedConnectionManager : IScopedConnectionManager
    {
        int _idCount;

        public event Action<IScopeInfo> ScopeDestroyed = i => { };

        // TODO: Reusing ID may be smart at some point
        public IScopeInfo CreateScope() => new ScopeInfo() { ConnectionID = _idCount++ };
        public void DestroyScope(IScopeInfo info) => ScopeDestroyed(info);

        class ScopeInfo : IScopeInfo
        {
            public int ConnectionID { get; init; }
        }
    }
}
