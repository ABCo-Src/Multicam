using ABCo.Multicam.Server.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Clients
{
    public interface IConnectedClientsManager
    {
        int NewConnectionId();
        IClientSyncedDataStore NewClientsDataNotifier(IServerTarget target);
        void OnClientDisconnected(IClientInfo info);

        event Action<IClientInfo> ClientDisconnected;
    }

    public class ConnectedClientsManager : IConnectedClientsManager
    {
        IServerInfo _info;
        int _idCount;

        public event Action<IClientInfo> ClientDisconnected = i => { };

        public ConnectedClientsManager(IServerInfo info) => _info = info;

        public IClientSyncedDataStore NewClientsDataNotifier(IServerTarget target)
        {
            // Create a client notifier
            var notifier = _info.Get<IClientSyncedDataStoreWithClientsManagementBinding, IServerTarget>(target);

            // Register it so it's notified when a client is disconnected (and can remove all targets registered with that client)
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