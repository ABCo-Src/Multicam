using ABCo.Multicam.Core;
using ABCo.Multicam.Core.General;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ABCo.Multicam.Core
{
	public interface IClientService<T> { }
	public interface IClientService<T, T2> { }
	public interface IClientService<T, T2, T3> { }

	public interface IClientInfo
	{
		int ConnectionID { get; }
		IServerConnection ServerConnection { get; }
		IThreadDispatcher Dispatcher { get; }

		T Get<T>() where T : class;
		T Get<T, T1>(T1 param1) where T : class, IClientService<T1>;
		T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, IClientService<T1, T2>;
		T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, IClientService<T1, T2, T3>;
	}

    public interface IServerService<T> { }
	public interface IServerService<T, T2> { }
	public interface IServerService<T, T2, T3> { }

	public interface IServerInfo
	{
		IThreadDispatcher Dispatcher { get; }
		IConnectedClientsManager ClientsManager { get; }
		IServerConnection GetLocalClientConnection();

		T Get<T>() where T : class;
        T Get<T, T1>(T1 param1) where T : class, IServerService<T1>;
        T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, IServerService<T1, T2>;
        T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, IServerService<T1, T2, T3>;
    }

	public class ServerInfo : IServerInfo
	{
		IServerConnection _localClientConnection;

		public IThreadDispatcher Dispatcher { get; }
		public IConnectedClientsManager ClientsManager => Get<IConnectedClientsManager>();
		public IServerConnection GetLocalClientConnection() => _localClientConnection;

		public ServerInfo(IThreadDispatcher dispatcher, IServerConnection connection)
		{
			Dispatcher = dispatcher;
			_localClientConnection = connection;
		}

		public T Get<T>() where T : class
		{
			// Check if it's a singleton
			if (ServerWideServiceRegisterSingletonStore<T>.Factory != null)
			{
				if (ServerWideServiceRegisterSingletonStore<T>.Object != null)
					return (T)ServerWideServiceRegisterSingletonStore<T>.Object;
				else
				{
					var res = ServerWideServiceRegisterSingletonStore<T>.Factory(this);
					ServerWideServiceRegisterSingletonStore<T>.Object = res;
					return res;
				}
			}

			// Then it must be a transient
			if (ServerWideServiceRegisterTransientStore<T>.Factory == null) throw new Exception("Unregistered service requested!");
			
			return ((Func<IServerInfo, T>)ServerWideServiceRegisterTransientStore<T>.Factory)(this);
		}

		public T Get<T, T1>(T1 param1) where T : class, IServerService<T1>
		{
			var factory = ServerWideServiceRegisterTransientStore<T>.Factory ?? throw new Exception();
			var castedFactory = (Func<T1, IServerInfo, T>)factory;
			return castedFactory(param1, this);
		}

		public T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, IServerService<T1, T2>
		{
			var factory = ServerWideServiceRegisterTransientStore<T>.Factory ?? throw new Exception();
			var castedFactory = (Func<T1, T2, IServerInfo, T>)factory;
			return castedFactory(param1, param2, this);
		}

		public T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, IServerService<T1, T2, T3>
		{
			var factory = ServerWideServiceRegisterTransientStore<T>.Factory ?? throw new Exception();
			var castedFactory = (Func<T1, T2, T3, IServerInfo, T>)factory;
			return castedFactory(param1, param2, param3, this);
		}

		public void AddSingleton<T>(Func<IServerInfo, T> val)
			where T : class
			=> ServerWideServiceRegisterSingletonStore<T>.Factory = val;

		public void AddTransient<T>(Func<IServerInfo, T> f) where T : class
			=> ServerWideServiceRegisterTransientStore<T>.Factory = f;

		public void AddTransient<T, T1>(Func<T1, IServerInfo, T> factory) where T : IServerService<T1> =>
			ServerWideServiceRegisterTransientStore<T>.Factory = factory;

		public void AddTransient<T, T1, T2>(Func<T1, T2, IServerInfo, T> factory) where T : IServerService<T1, T2> =>
			ServerWideServiceRegisterTransientStore<T>.Factory = factory;

		public void AddTransient<T, T1, T2, T3>(Func<T1, T2, T3, IServerInfo, T> factory) where T : IServerService<T1, T2, T3> =>
			ServerWideServiceRegisterTransientStore<T>.Factory = factory;
	}

	internal static class ServerWideServiceRegisterSingletonStore<T>
	{
		public static object? Object = null;
		public static Func<IServerInfo, T>? Factory = null;
	}

	internal static class ServerWideServiceRegisterTransientStore<T>
	{
		public static Delegate? Factory = null;
	}
}