using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Server.Hosting.Clients
{
	public interface IClientNotifier<T> : IServerService<T>, IDisposable
	{
        void Notify(string? changedProp);
		TClient GetOrAddClientEndpoint<TClient>(IClientInfo info) where TClient : class, IClientDataNotificationTarget<T>;
	}

    public interface IClientNotifierWithManagementBinding<T> : IClientNotifier<T>
	{
        void OnClientDisconnect(IClientInfo obj);
        void SetDisposeAction(Action act);
    }

    public interface IClientDataNotificationTarget<T> : IClientService<Dispatched<T>>
	{
        void Init();
        void OnServerStateChange(string? changedProp);
    }

    public class ClientNotifier<T> : IClientNotifierWithManagementBinding<T>
	{
        readonly Dictionary<int, ScopePresenters> _registeredClients = new();
        readonly Dispatched<T> _target;

		Action _onDispose = () => { };

        public ClientNotifier(T component, IServerInfo info)
        {
			// Wrap the server component up in a dispatcher service, so any actions
            // the clients perform gets dispatched back to the server thread properly
			_target = new Dispatched<T>(component, info);
        }

		// === All methods on this class should be thread-safe, and work from any thread (server, client etc.) ===

		public void SetDisposeAction(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => _onDispose();

        public TClient GetOrAddClientEndpoint<TClient>(IClientInfo info) where TClient : class, IClientDataNotificationTarget<T>
        {
            lock (this)
            {
                // If there's nothing registered, add the item
                if (!_registeredClients.TryGetValue(info.ConnectionID, out ScopePresenters val))
                {
                    var newVal = info.Get<TClient, Dispatched<T>>(_target);

#if DEBUG
                    // Throw if creating the endpoint managed to cause one to be added, that should never happen.
                    if (_registeredClients.ContainsKey(info.ConnectionID)) throw new Exception("Endpoint added during creation of itself!");
#endif

                    _registeredClients.Add(info.ConnectionID, new ScopePresenters(info.Dispatcher, newVal));
                    newVal.Init();
                    newVal.OnServerStateChange(null);
                    return newVal;
                }

                // Otherwise, return it
                return (TClient)val.NotificationTarget;
            }
        }

		public void Notify(string? changedProp)
		{
			// Inform every item in the dictionary of his change
			lock (this)
                foreach (var item in _registeredClients)
                    item.Value.Dispatcher.Queue(() => item.Value.NotificationTarget.OnServerStateChange(changedProp));
	    }

		public void OnClientDisconnect(IClientInfo obj)
		{
            lock (this)
			    _registeredClients.Remove(obj.ConnectionID);
		}

		public record struct ScopePresenters(IThreadDispatcher Dispatcher, IClientDataNotificationTarget<T> NotificationTarget);
	}
}
