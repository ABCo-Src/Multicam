using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.Management
{
	public interface IDisconnectionManager
	{
		event Action ClientDisconnected;
	}

	public class DisconnectionManager : IDisconnectionManager
	{
		public event Action ClientDisconnected = () => { };
		public void OnClientDisconnect() => ClientDisconnected();
	}
}
