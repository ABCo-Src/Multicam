using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Server.Hosting.Clients
{
	public interface IRemoteDataStore
    {
		T GetData<T>() where T : ServerData;
		T GetOrAddClientEndpoint<T>(IClientInfo info) where T : class, IClientDataNotificationTarget;
    }

    public interface IClientSyncedDataStore : IRemoteDataStore, IServerService<IServerTarget>, IDisposable
    {
		void SetData<T>(ServerData data) where T : ServerData;
	}

    public interface IClientSyncedDataStoreWithClientsManagementBinding : IClientSyncedDataStore
    {
        void OnClientDisconnect(IClientInfo obj);
        void SetDisposeAction(Action act);
    }

    public interface IClientDataNotificationTarget : IClientService<IServerTarget>
    {
        void Init();
        void OnDataChange(ServerData obj);
    }

    public abstract class ServerData { }

    public class ClientSyncedDataStore : IClientSyncedDataStoreWithClientsManagementBinding
	{
		readonly Dictionary<Type, ServerData> _fragmentStore;
        readonly Dictionary<int, ScopePresenters> _registeredPresenters = new();
        readonly IServerTarget _dispatchedTarget;

		Action _onDispose = () => { };

        public ClientSyncedDataStore(IServerTarget target, IServerInfo info)
        {
            // Wrap the target up in a dispatcher service, so everything the clients ask for gets dispatched back to the server thread properly
            _fragmentStore = new();
			_dispatchedTarget = info.Get<IDispatchingServerTarget, IServerTarget>(target);
        }

        public void SetDisposeAction(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => _onDispose();

        // === This method should work from any thread (server, client etc.) ===
        public T GetOrAddClientEndpoint<T>(IClientInfo info) where T : class, IClientDataNotificationTarget
        {
            lock (this)
            {
                // If there's nothing registered, add the item
                if (!_registeredPresenters.TryGetValue(info.ConnectionID, out ScopePresenters val))
                {
                    var newVal = ConstructNew();
                    _registeredPresenters.Add(info.ConnectionID, new ScopePresenters(info.Dispatcher, new List<IClientDataNotificationTarget> { newVal }));
					InitTarget(newVal, info);
                    return newVal;
                }

                // If the item is in the registered list, return that
                for (int i = 0; i < val.Presenters.Count; i++)
                    if (val.Presenters[i] is T presenter)
                        return presenter;

                // Otherwise, add it to the list
                {
                    var newVal = ConstructNew();
                    val.Presenters.Add(newVal);
					InitTarget(newVal, info);
					return newVal;
                }

                T ConstructNew() => info.Get<T, IServerTarget>(_dispatchedTarget);
            }
        }

        void InitTarget(IClientDataNotificationTarget target, IClientInfo info)
        {
            // Initialize it natively
            target.Init();

            // Now *send it* every piece of data we currently have
            foreach (var item in _fragmentStore.Values)
				info.Dispatcher.Queue(() => target.OnDataChange(item));
        }

        public void OnClientDisconnect(IClientInfo obj) => _registeredPresenters.Remove(obj.ConnectionID);

        public record struct ScopePresenters(IThreadDispatcher Dispatcher, List<IClientDataNotificationTarget> Presenters);

        public T GetData<T>() where T : ServerData => (T)_fragmentStore[typeof(T)];
        public void SetData<T>(ServerData data) where T : ServerData
        {
            // Update the value
            if (!data.GetType().IsAssignableTo(typeof(T))) throw new Exception("Generic arguemnt does not match data given!");
            _fragmentStore[typeof(T)] = data;

            // Notify of the change
			lock (this)
			{
				foreach (var list in _registeredPresenters.Values)
				{
					// Thread safety: It's better to have the for inside the dispatch, as the only way the list can be
					// changed is *by* the client appending to it (which the loop is fine with), so we don't get cross-thread accesses like this
					var presenters = list.Presenters;
					list.Dispatcher.Queue(() =>
					{
						for (int i = 0; i < presenters.Count; i++)
							presenters[i].OnDataChange(data);
					});
				}
			}
		}
	}
}
