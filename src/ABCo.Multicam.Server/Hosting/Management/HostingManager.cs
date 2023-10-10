using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management.Data;
using System.Net;
using System.Net.Sockets;

namespace ABCo.Multicam.Server.Hosting.Management
{
	public interface IHostingManager : IServerTarget, IDisposable
    {
    }

    public class HostingManager : IHostingManager
    {
        public const int TOGGLE_ONOFF = 1;
		public const int SET_MODE = 2;
		public const int SET_CUSTOM_CONFIG = 3;

        readonly IServerInfo _info;
        readonly ILocalIPCollection _localIPAddresses;
		readonly IClientSyncedDataStore _data;
		INativeServerHost? _localNetworkHost;

        public IRemoteDataStore DataStore => _data;

        public HostingManager(IServerInfo info)
        {
            _info = info;
            _localIPAddresses = info.Get<ILocalIPCollection, Action>(HandleIPCollectionChange);
            _data = info.ClientsManager.NewClientsDataNotifier(this);
            _data.SetData<HostingConfigMode>(new HostingConfigMode(true));
            _data.SetData<HostingCustomModeConfig>(new HostingCustomModeConfig(new string[] { "http://127.0.0.1:800" }));
            _data.SetData<HostingActiveConfig>(new HostingActiveConfig(null));
            _data.SetData<HostingExecutionStatus>(new HostingExecutionStatus(false));
        }

		void UpdateCurrentlyActiveConfig()
		{
			if (_data.GetData<HostingConfigMode>().IsAutomatic) 
                SetActiveConfigToAutomaticValue();
			else
			{
				var config = _data.GetData<HostingCustomModeConfig>();
				_data.SetData<HostingActiveConfig>(new HostingActiveConfig(config.HostNames.Count == 0 ? null : config.HostNames[0]));
			}
		}

		void HandleIPCollectionChange()
        {
            // If we're in the automatic mode, then this impacts our current config.
            if (_data.GetData<HostingConfigMode>().IsAutomatic)
                SetActiveConfigToAutomaticValue();
        }

        void SetActiveConfigToAutomaticValue()
        {
            // Try to get the currently-loaded local IP addresses (may fail if they're not loaded or there's no network connection)
            var localAddresses = _localIPAddresses.GetLoadedAddresses();
            if (localAddresses == null || localAddresses.Length == 0) 
            {
                SetUnusable();
                return;
            }

            // Try to get the best IP address
			var bestIP = SelectBestAutomaticIP(localAddresses);
            if (bestIP == null)
            {
                SetUnusable();
                return;
            }

            // If successful, setup the config
            _data.SetData<HostingActiveConfig>(new HostingActiveConfig(bestIP.AddressFamily switch
			{
				AddressFamily.InterNetwork => $"http://{bestIP}:800",
				AddressFamily.InterNetworkV6 => $"http://[{bestIP}]:800",
				_ => throw new Exception()
			}));

            void SetUnusable() => _data.SetData<HostingActiveConfig>(new HostingActiveConfig(null));
		}

		static IPAddress? SelectBestAutomaticIP(IPAddress[] ips)
        {
			// If there are any IPv4s with "192.168.???", this is probably the best bet.
            for (int i = 0; i < ips.Length; i++)
				if (ips[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    var bytes = ips[i].GetAddressBytes();
                    if (bytes[0] == 192 && bytes[1] == 128)
                        return ips[i];
                }

            // If not, let's take the last IPv4 address.
            for (int i = ips.Length - 1; i >= 0; i--)
                if (ips[i].AddressFamily == AddressFamily.InterNetwork)
                    return ips[i];

			// And if that doesn't work, then we'll just take the last IPv6 address
			for (int i = ips.Length - 1; i >= 0; i--)
				if (ips[i].AddressFamily == AddressFamily.InterNetworkV6)
					return ips[i];

            return null;
		}

		public async void PerformAction(int id) 
        {
            if (id == TOGGLE_ONOFF)
            {
                // If not initialized, initialize now.
				if (_data.GetData<HostingExecutionStatus>().IsConnected)
                {
                    await _localNetworkHost!.Stop();
                    await _localNetworkHost.DisposeAsync();
                    _localNetworkHost = null;
					_data.SetData<HostingExecutionStatus>(new HostingExecutionStatus(false));
				}
                else
                {
                    // TODO: Error handling
                    // TODO: Slam protection - make synchronous and put server on background thread?
                    var activeConfig = _data.GetData<HostingActiveConfig>();
					if (activeConfig.HostName == null) throw new Exception("Cannot start server with unstartable settings.");

                    // Create/start a new host
                    _localNetworkHost = _info.Get<INativeServerHost, NativeServerHostConfig>(new(activeConfig.HostName));
					await _localNetworkHost.Start();

					_data.SetData<HostingExecutionStatus>(new HostingExecutionStatus(true));
				}
			}
        }

		public void PerformAction(int id, object param)
        {
            if (id == SET_MODE)
				_data.SetData<HostingConfigMode>((HostingConfigMode)param);
			else if (id == SET_CUSTOM_CONFIG)
			    _data.SetData<HostingCustomModeConfig>((HostingCustomModeConfig)param);

			UpdateCurrentlyActiveConfig();
		}

		public void Dispose() => _localIPAddresses.Dispose();
	}
}