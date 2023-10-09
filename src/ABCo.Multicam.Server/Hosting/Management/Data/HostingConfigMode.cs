using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Management.Data
{
	public class HostingConfigMode : ServerData
	{
		public bool IsAutomatic { get; }
		public HostingConfigMode(bool isAutomatic) => IsAutomatic = isAutomatic;
	}
}
