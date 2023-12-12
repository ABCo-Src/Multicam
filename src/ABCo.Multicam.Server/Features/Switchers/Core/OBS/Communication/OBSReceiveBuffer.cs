using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
	public class OBSReceiveBuffer
	{
		public byte[] Buffer;

		public OBSReceiveBuffer(byte[] buffer) => Buffer = buffer;
	}
}
