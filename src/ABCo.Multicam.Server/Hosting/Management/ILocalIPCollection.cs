using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Management
{
	/// <summary>
	/// Provides notifications to the given action when the local IP collection updates
	/// </summary>
	public interface ILocalIPCollection : IServerService<Action>, IDisposable
	{
		IPAddress[]? GetLoadedAddresses();
	}
}
