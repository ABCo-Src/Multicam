using ABCo.Multicam.Server.Hosting.Management;
using System.Net;

namespace ABCo.Multicam.App.Win32.Services
{
	public class AvailableIPCollection : ILocalIPCollection
	{
		IPAddress[]? _currentlyLoadedAddresses = null;

		public AvailableIPCollection(Action notifyUpdate) => SetupAddresses(notifyUpdate);
		async void SetupAddresses(Action notifyUpdate)
		{
			_currentlyLoadedAddresses = await Dns.GetHostAddressesAsync(Dns.GetHostName());
			notifyUpdate();
		}

		public IPAddress[]? GetLoadedAddresses() => _currentlyLoadedAddresses;

		public void Dispose() { }
	}
}
