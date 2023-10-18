﻿using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting.Management.Data
{
	public class HostingActiveConfig : ServerData
	{
		// Null if uninitialized
		public string? HostName { get; }
		public HostingActiveConfig(string? hostName) => HostName = hostName;
	}
}