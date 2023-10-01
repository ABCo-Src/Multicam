using ABCo.Multicam.Server;
using ABCo.Multicam.Server.General;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ABCo.Multicam.UI
{
    public class ClientServices : IClientInfo
    {
        Dictionary<Type, Delegate> _transientDict = new();
        IServiceProvider _provider;

		internal ClientServices(IServiceProvider provider, int connectionId, IThreadDispatcher dispatcher, IServerConnection serverConnection, Dictionary<Type, Delegate> transientDict)
		{
            Dispatcher = dispatcher;
            ConnectionID = connectionId;
            ServerConnection = serverConnection;
			_provider = provider;
			_transientDict = transientDict;
		}

		public int ConnectionID { get; }
		public IThreadDispatcher Dispatcher { get; }
        public IServerConnection ServerConnection { get; }

		public T Get<T>() where T : class
        {
            // Check if it's a transient
            if (_transientDict.TryGetValue(typeof(T), out Delegate? val))
                return ((Func<IClientInfo, T>)val)(this);

            // Otherwise, it must be in the base container
            return _provider.GetService<T>() ?? throw new Exception("Unregistered service requested!");
        }

        public T Get<T, T1>(T1 param1) where T : class, IClientService<T1>
        {
            if (!_transientDict.TryGetValue(typeof(T), out Delegate? val)) throw new Exception("Unregistered service requested!");

            var castedFactory = (Func<T1, IClientInfo, T>)val;
            return castedFactory(param1, this);
        }

        public T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, IClientService<T1, T2>
        {
            if (!_transientDict.TryGetValue(typeof(T), out Delegate? val)) throw new Exception("Unregistered service requested!");

            var castedFactory = (Func<T1, T2, IClientInfo, T>)val;
            return castedFactory(param1, param2, this);
        }

        public T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, IClientService<T1, T2, T3>
        {
            if (!_transientDict.TryGetValue(typeof(T), out Delegate? val)) throw new Exception("Unregistered service requested!");

            var castedFactory = (Func<T1, T2, T3, IClientInfo, T>)val;
            return castedFactory(param1, param2, param3, this);
        }
    }

    public class ClientServicesBuilder
    {
        Dictionary<Type, Delegate> _transientDict = new();
        IServiceCollection _collection;
        bool _registerScopedAsSingletons;

        public ClientServicesBuilder(IServiceCollection collection, bool registerScopedAsSingletons = false)
        {
            _collection = collection;
            _registerScopedAsSingletons = registerScopedAsSingletons;
        }

        public void AddSingleton<T>(T val) where T : class => _collection.AddSingleton(val);
        public void AddScoped<T, TTarget>() where T : class where TTarget : class, T
        {
            if (_registerScopedAsSingletons)
                _collection.AddSingleton<T, TTarget>();
            else
                _collection.AddScoped<T, TTarget>();
        }

        public void AddTransient<T>(Func<IClientInfo, T> f) where T : class => _transientDict.Add(typeof(T), f);

        public void AddTransient<T, T1>(Func<T1, IClientInfo, T> factory) where T : IClientService<T1> =>
            _transientDict.Add(typeof(T), factory);

        public void AddTransient<T, T1, T2>(Func<T1, T2, IClientInfo, T> factory) where T : IClientService<T1, T2> =>
            _transientDict.Add(typeof(T), factory);

        public void AddTransient<T, T1, T2, T3>(Func<T1, T2, T3, IClientInfo, T> factory) where T : IClientService<T1, T2, T3> =>
            _transientDict.Add(typeof(T), factory);

        public IClientInfo Build(IServiceProvider provider, IThreadDispatcher dispatcher, IServerConnection server, int id) => new ClientServices(provider, id, dispatcher, server, _transientDict);
    }
}
