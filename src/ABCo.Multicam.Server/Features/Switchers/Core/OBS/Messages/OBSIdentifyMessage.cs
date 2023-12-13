using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
	public class OBSIdentifyMessageWithAuth : OBSIdentifyMessage
	{
		[JsonPropertyName("authentication"), JsonRequired]
		public string Authentication { get; set; } = null!;
	}

	public class OBSIdentifyMessage
	{
		[JsonPropertyName("rpcVersion"), JsonRequired]
		public int RPCVersion { get; set; }

		[JsonPropertyName("eventSubscriptions"), JsonRequired]
		public int EventSubscriptions { get; set; }
	}
}
