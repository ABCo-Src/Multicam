using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Management.Data
{
	public class HostingConfig : ServerData
	{
		public string[] HostNames { get; }
		public HostingConfig(string[] hostNames) => HostNames = hostNames;
	}

	public class HostingSourceConfig : ServerData
	{
		public string HostName { get; }
		public HostingSourceConfig(string hostName) => HostName = hostName;
	}
}
