using ABCo.Multicam.Server.General;
using System.Data;

namespace ABCo.Multicam.Server.Hosting.Clients
{
	public interface IDispatched<T>
    {
        void CallDispatched(Action<T> act);
        Dispatched<TNew> CastTo<TNew>() where TNew : T;
	}

    public sealed class Dispatched<T> : IDispatched<T>
    {
		readonly T _native;
		readonly IThreadDispatcher _dispatcher;

        public Dispatched(T native, IServerInfo info) : this(native, info.Dispatcher) { }
        private Dispatched(T native, IThreadDispatcher dispatcher)
        {
			_native = native;
			_dispatcher = dispatcher;
		}

		public void CallDispatched(Action<T> act) => _dispatcher.Queue(() => act(_native));
		public TVal Get<TVal>(Func<T, TVal> get) => get(_native); // IMPORTANT NOTE: We trust property getters to be safe for perf... They better be!
		public Dispatched<TNew> CastTo<TNew>() where TNew : T => new Dispatched<TNew>((TNew)_native!, _dispatcher);
	}
}
