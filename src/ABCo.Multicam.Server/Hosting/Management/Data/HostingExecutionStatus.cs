using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Management.Data
{
	public class HostingExecutionStatus : ServerData
	{
		public bool IsConnected { get; }
		public HostingExecutionStatus(bool isConnected) => IsConnected = isConnected;
	}
}
