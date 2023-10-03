using ABCo.Multicam.Server.Hosting.Clients;
using System.Net;

namespace ABCo.Multicam.Server.Hosting.Management
{
	public interface IHostingManager : IServerTarget
    {
    }

    public class HostingManager : IHostingManager
    {
        public const int UPDATE_CONFIG = 1;

        IActiveServerHost _localNetworkHost;

        public IRemoteDataStore DataStore => throw new NotImplementedException();

        public void PerformAction(int id)
        {
            throw new NotImplementedException();
        }

        public void PerformAction(int id, object param)
        {
            throw new NotImplementedException();
        }

        public void RefreshData<T>() where T : ServerData
        {
            throw new NotImplementedException();
        }

        void UpdateConfiguration(IPAddress address)
        {

        }

    }
}
