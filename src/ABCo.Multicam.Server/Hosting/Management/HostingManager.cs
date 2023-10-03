using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Management
{
    public interface IHostingManager : IServerTarget
    {
    }

    public class HostingManager : IHostingManager
    {
        public const int UPDATE_CONFIG = 1;

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
