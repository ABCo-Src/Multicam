using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management.Data;
using System.Net;

namespace ABCo.Multicam.Server.Hosting.Management
{
	public interface IHostingManager : IServerTarget
    {
    }

    public class HostingManager : IHostingManager
    {
        public const int TOGGLE_ONOFF = 1;
		public const int SET_CONFIG = 2;

        readonly IServerInfo _info;
		readonly IClientSyncedDataStore _data;
		INativeServerHost? _localNetworkHost;
        bool _isConnected;

        public IRemoteDataStore DataStore => _data;

        public HostingManager(IServerInfo info)
        {
            _info = info;
            _data = info.Get<IClientSyncedDataStore>();
        }

        public async void PerformAction(int id) 
        {
            if (id == TOGGLE_ONOFF)
            {
                // If not initialized, initialize now.
                _localNetworkHost ??= _info.Get<INativeServerHost>();

				if (_isConnected)
                {
                    await _localNetworkHost.Stop();
					_isConnected = false;
				}
                else
                {
                    // TODO: Error handling
                    // TODO: Slam protection - make synchronous and put server on background thread?
					await _localNetworkHost.Start(_data.GetData<HostingConfig>());
					_isConnected = true;
				}
			}
        }

        public void PerformAction(int id, object param)
        {
            if (id == SET_CONFIG)
                _data.SetData<HostingConfig>((HostingConfig)param);
        }
    }
}
