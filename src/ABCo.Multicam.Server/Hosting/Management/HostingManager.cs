using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using System.Net;
using System.Net.Sockets;

namespace ABCo.Multicam.Server.Hosting.Management
{
	public interface IHostingManager : IServerComponent, IDisposable
    {
		IClientNotifier<IHostingManagerState, IHostingManager> ClientNotifier { get; }
		void ToggleOnOff();
        void SetMode(bool isAutomatic);
        void SetCustomModeConfig(string[] customModeConfig);
	}

    public class HostingManager : IHostingManager
    {
        public const int TOGGLE_ONOFF = 1;
		public const int SET_MODE = 2;
		public const int SET_CUSTOM_CONFIG = 3;

        readonly IHostingManagerState _state;
        readonly IServerInfo _info;
        readonly ILocalIPCollection _localIPAddresses;
		INativeServerHost? _localNetworkHost;

		public IClientNotifier<IHostingManagerState, IHostingManager> ClientNotifier => _state.ClientNotifier;

		public HostingManager(IServerInfo info)
        {
            _info = info;
            _localIPAddresses = info.Get<ILocalIPCollection, Action>(HandleIPCollectionChange);
			_state = info.Get<IHostingManagerState, IHostingManager>(this);
        }

		void UpdateCurrentlyActiveConfig()
		{
			if (_state.IsAutomatic)
                SetActiveConfigToAutomaticValue();
			else
			{
				var config = _state.CustomModeHostNames;
				_state.ActiveHostName = config.Count == 0 ? null : config[0];
			}
		}

		void HandleIPCollectionChange()
        {
            // If we're in the automatic mode, then this impacts our current config.
            if (_state.IsAutomatic)
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
            _state.ActiveHostName = bestIP.AddressFamily switch
			{
				AddressFamily.InterNetwork => $"http://{bestIP}:800",
				AddressFamily.InterNetworkV6 => $"http://[{bestIP}]:800",
				_ => throw new Exception()
			};

            void SetUnusable() => _state.ActiveHostName = null;
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

		public async void ToggleOnOff()
        {
            // If not initialized, initialize now.
			if (_state.IsConnected)
            {
                await _localNetworkHost!.Stop();
                await _localNetworkHost.DisposeAsync();
                _localNetworkHost = null;
				_state.IsConnected = false;
			}
            else
            {
                // TODO: Error handling
                // TODO: Slam protection - make synchronous and put server on background thread?
                var activeConfig = _state.ActiveHostName ?? throw new Exception("Cannot start server with unstartable settings.");

				// Create/start a new host
				_localNetworkHost = _info.Get<INativeServerHost, NativeServerHostConfig>(new(activeConfig));
				await _localNetworkHost.Start();

                _state.IsConnected = true;
			}
        }

		public void SetMode(bool isAutomatic)
		{
			_state.IsAutomatic = isAutomatic;
			UpdateCurrentlyActiveConfig();
		}

		public void SetCustomModeConfig(string[] customModeConfig)
		{
			_state.CustomModeHostNames = customModeConfig;
			UpdateCurrentlyActiveConfig();
		}

		public void Dispose() => _localIPAddresses.Dispose();
	}
}