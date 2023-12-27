using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.General.Factories;
using ABCo.Multicam.Server.Hosting.Management;

namespace ABCo.Multicam.Server
{
    public interface IServerService<T> { }
	public interface IServerService<T, T2> { }
	public interface IServerService<T, T2, T3> { }

	public interface IServerInfo
	{
		IServerFactories Factories { get; }
		IServerSharedServices Shared { get; }
		IPlatformInfo PlatformInfo { get; }
		IThreadDispatcher Dispatcher { get; }
	}

	public class ServerInfo : IServerInfo
	{
		public IServerFactories Factories { get; }
		public IServerSharedServices Shared { get; }
		public IPlatformInfo PlatformInfo { get; }
		public IThreadDispatcher Dispatcher { get; }

		public ServerInfo(IThreadDispatcher dispatcher, Func<NativeServerHostConfig, IServerInfo, INativeServerHost> createServerHost, 
			Func<Action, ILocalIPCollection> createIPCollection, IPlatformInfo platInfo)
		{
			Factories = new ServerFactories(new HostingFactory(createServerHost, createIPCollection, this), this);
			Shared = new ServerSharedServices(this);
			Dispatcher = dispatcher;
			PlatformInfo = platInfo;
		}
	}
}