using ABCo.Multicam.Server;
using ABCo.Multicam.Server.General;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.Client
{
	public interface IClientInfo : IDisposable
	{
		int ConnectionID { get; }
		IMulticamServer ServerConnection { get; }
		ISharedVMs Shared { get; }
		IThreadDispatcher Dispatcher { get; }

        event Action ClientDisconnected;
	}

	public class ClientInfo : IClientInfo
    {
		public ClientInfo(IThreadDispatcher dispatcher, IMulticamServer serverConnection)
		{
            Dispatcher = dispatcher;
            ServerConnection = serverConnection;
            Shared = new SharedVMs(this);
		}

		public int ConnectionID { get; }
        public ISharedVMs Shared { get; }
		public IThreadDispatcher Dispatcher { get; }
        public IMulticamServer ServerConnection { get; }

		public event Action ClientDisconnected = () => { };

        public void Dispose() => ClientDisconnected();
	}
}
