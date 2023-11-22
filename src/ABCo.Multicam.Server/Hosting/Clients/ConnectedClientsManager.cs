namespace ABCo.Multicam.Server.Hosting.Clients
{
	public interface IConnectedClientsManager
    {
        int NewConnectionId();
        IClientNotifier<T> NewClientsDataNotifier<T>(T component);
        void OnClientDisconnected(IClientInfo info);

        event Action<IClientInfo> ClientDisconnected;
    }

    public class ConnectedClientsManager : IConnectedClientsManager
    {
		readonly IServerInfo _info;
        int _idCount;

        public event Action<IClientInfo> ClientDisconnected = i => { };

        public ConnectedClientsManager(IServerInfo info) => _info = info;

        public IClientNotifier<T> NewClientsDataNotifier<T>(T component)
        {
            // Create a client notifier
            var notifier = new ClientNotifier<T>(component, _info);

            // Register it so it's notified when a client is disconnected (and can remove the target registered with that client in response)
            ClientDisconnected += notifier.OnClientDisconnect;

            // Set it so it gets deregistered when disposed
            notifier.SetDisposeAction(() => ClientDisconnected -= notifier.OnClientDisconnect);
            return notifier;
        }

        public void OnClientDisconnected(IClientInfo info) => ClientDisconnected(info);

        // TODO: Reusing ID may be smart at some point
        public int NewConnectionId() => _idCount++;
    }
}