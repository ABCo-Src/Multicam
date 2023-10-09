using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Management.Data
{
	public class HostingCustomModeConfig : ServerData
	{
		public IReadOnlyList<string> HostNames { get; }
		public HostingCustomModeConfig(string[] hostNames) => HostNames = hostNames;
	}
}
