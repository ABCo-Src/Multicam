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
		IServerConnection Server { get; }
		ISharedVMs Shared { get; }
		IThreadDispatcher Dispatcher { get; }
		IDisconnectionManager DisconnectionManager { get; }
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
		public ClientInfo(IThreadDispatcher dispatcher, IServerConnection serverConnection, ISharedVMs? shared = null, DisconnectionManager? discManager = null)
		{
            Dispatcher = dispatcher;
            Server = serverConnection;
            Shared = shared ?? new SharedVMs(this);
			_disconnectionManager = discManager ?? new DisconnectionManager();
		}

        public ISharedVMs Shared { get; }
		public IThreadDispatcher Dispatcher { get; }
        public IServerConnection Server { get; }
		public IDisconnectionManager DisconnectionManager => _disconnectionManager;

		public IFrameClientInfo NewFrameClientInfo(IFrameVM frame) => new FrameClientInfo(this, frame);
		public void Dispose() => _disconnectionManager.OnClientDisconnect();

		public class FrameClientInfo : ClientInfo, IFrameClientInfo
		{
			public FrameClientInfo(ClientInfo copyFrom, IFrameVM frame) : 
				base(copyFrom.Dispatcher, copyFrom.Server, copyFrom.Shared, copyFrom._disconnectionManager)
			{
				Frame = frame;
			}

			public IFrameVM Frame { get; }
		}
	}
}
