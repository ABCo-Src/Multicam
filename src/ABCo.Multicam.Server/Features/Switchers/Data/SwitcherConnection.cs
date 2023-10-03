using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features.Switchers.Data
{
	public class SwitcherConnection : ServerData
	{
		public bool IsConnected { get; }
		public SwitcherConnection(bool isConnected) => IsConnected = isConnected;
	}
}
