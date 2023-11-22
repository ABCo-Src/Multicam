using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Server.Hosting.Clients
{
	public interface IClientNotifier<TServerComponentState, TServerComponent> : IServerService<IServerComponent>, IDisposable
	{
        void Notify(string? changedProp);
		TClient GetOrAddClientEndpoint<TClient>(IClientInfo info) where TClient : class, IClientDataNotificationTarget<TServerComponentState, TServerComponent>;
	}

    public interface IClientNotifierWithManagementBinding<TServerComponentState, TServerComponent> : IClientNotifier<TServerComponentState, TServerComponent>
	{
        void OnClientDisconnect(IClientInfo obj);
        void SetDisposeAction(Action act);
    }

    public interface IClientDataNotificationTarget<TServerComponentState, TServerComponent> : IClientService<TServerComponentState, IDispatchedServerComponent<TServerComponent>>
	{
        void Init();
        void OnServerStateChange(string? changedProp);
    }

    public abstract class ServerData { }

    public class ClientNotifier<TServerComponentState, TServerComponent> : IClientNotifierWithManagementBinding<TServerComponentState, TServerComponent>
	{
        readonly Dictionary<int, ScopePresenters> _registeredClients = new();
        readonly IDispatchedServerComponent<TServerComponent> _dispatchedTarget;
        readonly TServerComponentState _state;

		Action _onDispose = () => { };

        public ClientNotifier(TServerComponent serverComponent, TServerComponentState data, IServerInfo info)
        {
			// Wrap the server component up in a dispatcher service, so any actions
            // the clients perform gets dispatched back to the server thread properly
			_dispatchedTarget = new DispatchedServerComponent<TServerComponent>(serverComponent, info);
			_state = data;
        }

		// === All methods on this class should be thread-safe, and work from any thread (server, client etc.) ===

		public void SetDisposeAction(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => _onDispose();

        public TClient GetOrAddClientEndpoint<TClient>(IClientInfo info) where TClient : class, IClientDataNotificationTarget<TServerComponentState, TServerComponent>
        {
            lock (this)
            {
                // If there's nothing registered, add the item
                if (!_registeredClients.TryGetValue(info.ConnectionID, out ScopePresenters val))
                {
                    var newVal = info.Get<TClient, TServerComponentState, IDispatchedServerComponent<TServerComponent>>(_state, _dispatchedTarget);
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

		public record struct ScopePresenters(IThreadDispatcher Dispatcher, IClientDataNotificationTarget<TServerComponentState, TServerComponent> NotificationTarget);
	}
}
