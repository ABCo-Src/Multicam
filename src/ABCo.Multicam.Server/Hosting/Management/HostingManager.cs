using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace ABCo.Multicam.Server.Hosting.Management
{
	public interface IHostingManager : IDisposable
    {
		bool IsAutomatic { get; }
		IReadOnlyList<string> CustomModeHostNames { get; }
		string? ActiveHostName { get; }
		bool IsConnected { get; }

		IClientNotifier<IHostingManager> ClientNotifier { get; }
		void ToggleOnOff();
        void SetMode(bool isAutomatic);
        void SetCustomModeConfig(string[] customModeConfig);
	}

    public partial class HostingManager : BindableServerComponent<IHostingManager>, IHostingManager
    {
		[ObservableProperty] bool _isAutomatic = true;
		[ObservableProperty] string? _activeHostName;
		[ObservableProperty] IReadOnlyList<string> _customModeHostNames = new string[] { "http://127.0.0.1:800" };
		[ObservableProperty] bool _isConnected;

        readonly IServerInfo _info;
        readonly ILocalIPCollection _localIPAddresses;
		INativeServerHost? _localNetworkHost;

		public HostingManager(IServerInfo info) : base(info)
        {
            _info = info;
            _localIPAddresses = info.Get<ILocalIPCollection, Action>(HandleIPCollectionChange);
        }

		void UpdateCurrentlyActiveConfig()
		{
			if (IsAutomatic)
                SetActiveConfigToAutomaticValue();
			else
			{
				var config = CustomModeHostNames;
				ActiveHostName = config.Count == 0 ? null : config[0];
			}
		}

		void HandleIPCollectionChange()
        {
            // If we're in the automatic mode, then this impacts our current config.
            if (IsAutomatic)
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
            ActiveHostName = bestIP.AddressFamily switch
			{
				AddressFamily.InterNetwork => $"http://{bestIP}:800",
				AddressFamily.InterNetworkV6 => $"http://[{bestIP}]:800",
				_ => throw new Exception()
			};

            void SetUnusable() => ActiveHostName = null;
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
			if (IsConnected)
            {
                await _localNetworkHost!.Stop();
                await _localNetworkHost.DisposeAsync();
                _localNetworkHost = null;
				IsConnected = false;
			}
            else
            {
                // TODO: Error handling
                // TODO: Slam protection - make synchronous and put server on background thread?
                var activeConfig = ActiveHostName ?? throw new Exception("Cannot start server with unstartable settings.");

				// Create/start a new host
				_localNetworkHost = _info.Get<INativeServerHost, NativeServerHostConfig>(new(activeConfig));
				await _localNetworkHost.Start();

                IsConnected = true;
			}
        }

		public void SetMode(bool isAutomatic)
		{
			IsAutomatic = isAutomatic;
			UpdateCurrentlyActiveConfig();
		}

		public void SetCustomModeConfig(string[] customModeConfig)
		{
			CustomModeHostNames = customModeConfig;
			UpdateCurrentlyActiveConfig();
		}

		public override void DisposeComponent() => _localIPAddresses.Dispose();
	}
}