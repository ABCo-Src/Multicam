using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting;

namespace ABCo.Multicam.Server.Features.Switchers.Data
{
    public class SwitcherConnection : ServerData
	{
		public bool IsConnected { get; }
		public SwitcherConnection(bool isConnected) => IsConnected = isConnected;
	}
}
