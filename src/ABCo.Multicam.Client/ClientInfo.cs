using ABCo.Multicam.Client.Management;
using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Server;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.Client
{
    public interface IClientInfo 
	{
		IMulticamServer ServerConnection { get; }
		ISharedVMs Shared { get; }
		IThreadDispatcher Dispatcher { get; }
		IDisconnectionManager DisconnectionManager { get; }
		Dispatched<T> CreateServerDispatcher<T>(T item);
		IFrameClientInfo NewFrameClientInfo(IFrameVM frame);
	}

	public interface IFrameClientInfo : IClientInfo, IDisposable
	{
		IFrameVM Frame { get; }
	}

	public class ClientInfo : IClientInfo
    {
		DisconnectionManager _disconnectionManager;

		// Creates a global client info
		public ClientInfo(IThreadDispatcher dispatcher, IMulticamServer serverConnection, ISharedVMs? shared = null, DisconnectionManager? discManager = null)
		{
            Dispatcher = dispatcher;
            ServerConnection = serverConnection;
            Shared = shared ?? new SharedVMs(this);
			_disconnectionManager = discManager ?? new DisconnectionManager();
		}

        public ISharedVMs Shared { get; }
		public IThreadDispatcher Dispatcher { get; }
        public IMulticamServer ServerConnection { get; }
		public IDisconnectionManager DisconnectionManager => _disconnectionManager;

		public IFrameClientInfo NewFrameClientInfo(IFrameVM frame) => new FrameClientInfo(this, frame);
		public Dispatched<T> CreateServerDispatcher<T>(T item) => new Dispatched<T>(item, ServerConnection);
		public void Dispose() => _disconnectionManager.OnClientDisconnect();

		public class FrameClientInfo : ClientInfo, IFrameClientInfo
		{
			public FrameClientInfo(ClientInfo copyFrom, IFrameVM frame) : 
				base(copyFrom.Dispatcher, copyFrom.ServerConnection, copyFrom.Shared, copyFrom._disconnectionManager)
			{
				Frame = frame;
			}

			public IFrameVM Frame { get; }
		}
	}
}
