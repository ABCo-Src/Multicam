using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
	public class SwitcherConnection : ServerData
	{
		public bool IsConnected { get; }
		public SwitcherConnection(bool isConnected) => IsConnected = isConnected;
	}
}
