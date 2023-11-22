using ABCo.Multicam.Server.General;
using System.Data;

namespace ABCo.Multicam.Server.Hosting.Clients
{
	public interface IDispatchedServerComponent<T> : IServerService<IServerComponent> 
    {
        void CallDispatched(Action<T> act);
        IDispatchedServerComponent<TNew> CastTo<TNew>() where TNew : T;
	}

    public class DispatchedServerComponent<T> : IDispatchedServerComponent<T>
    {
		readonly T _native;
		readonly IThreadDispatcher _dispatcher;

        public DispatchedServerComponent(T native, IServerInfo info) : this(native, info.Dispatcher) { }
        private DispatchedServerComponent(T native, IThreadDispatcher dispatcher)
        {
			_native = native;
			_dispatcher = dispatcher;
		}

		public void CallDispatched(Action<T> act) => _dispatcher.Queue(() => act(_native));

		public IDispatchedServerComponent<TNew> CastTo<TNew>() where TNew : T => new DispatchedServerComponent<TNew>((TNew)_native!, _dispatcher);
	}
}
