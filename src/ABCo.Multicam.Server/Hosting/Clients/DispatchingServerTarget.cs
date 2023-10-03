using ABCo.Multicam.Server.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Clients
{
    public interface IDispatchingServerTarget : IServerTarget, IServerService<IServerTarget> { }
    public class DispatchingServerTarget : IDispatchingServerTarget
    {
        IServerTarget _native;
        IThreadDispatcher _dispatcher;

        public DispatchingServerTarget(IServerTarget native, IServerInfo service)
        {
            _native = native;
            _dispatcher = service.Dispatcher;
        }

        // We don't dispatch the client notifier because it's thread-safe
        public IRemoteDataStore DataStore => _native.DataStore;

        public void PerformAction(int id) => _dispatcher.Queue(() => _native.PerformAction(id));
        public void PerformAction(int id, object param) => _dispatcher.Queue(() => _native.PerformAction(id, param));
    }
}
