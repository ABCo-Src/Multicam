using ABCo.Multicam.Server.Features.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting
{
    public interface IServerTarget
    {
        IRemoteClientNotifier ClientMessageDispatcher { get; }
        void PerformAction(int id);
        void PerformAction(int id, object param);
        void RefreshData<T>() where T : ServerData;
    }
}
