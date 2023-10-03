using ABCo.Multicam.Server.Features.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Clients
{
    public interface IServerTarget
    {
        IRemoteDataStore DataStore { get; }
        void PerformAction(int id);
        void PerformAction(int id, object param);
    }
}
