using ABCo.Multicam.Server.General;

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

        public Dispatched(T native, IThreadDispatcher dispatcher)
        {
			_native = native;
			_dispatcher = dispatcher;
		}

		public void CallDispatched(Action<T> act) => _dispatcher.Queue(() => act(_native));
		public void CallDispatchedAndUnpack<T2>(Dispatched<T2> toUnpack, Action<T, T2> act) => _dispatcher.Queue(() => act(_native, toUnpack._native));
		public TVal Get<TVal>(Func<T, TVal> get) => get(_native); // IMPORTANT NOTE: We trust property getters to be dispatcher-safe for perf... They better be!
		public Dispatched<TNew> CastTo<TNew>() where TNew : T => new((TNew)_native!, _dispatcher);
	}
}
