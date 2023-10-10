using ABCo.Multicam.Server.Hosting.Management;
using System.Net;

namespace ABCo.Multicam.UI.Blazor.Web.Services
{
	public class LocalIPCollection : ILocalIPCollection
	{
		public IPAddress[]? GetLoadedAddresses() => Array.Empty<IPAddress>();
		public void Dispose() { }
	}
}
