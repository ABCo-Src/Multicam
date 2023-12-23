using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS
{
	public class OBSCommunicationException : Exception
	{
		public bool IsDisconnectionException { get; }
		public static OBSCommunicationException UnexpectedDisconnection => new("Unexpected disconnection from OBS.", true);
		public OBSCommunicationException(string msg, bool isDisconnectionException = false) : base(msg) => IsDisconnectionException = isDisconnectionException;
	}
}
