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
		[JsonPropertyName("authentication")]
		public string? Authentication { get; set; }
	}

	public class OBSIdentifyMessage
	{
		[JsonPropertyName("rpcVersion")]
		public int RPCVersion { get; set; }

		[JsonPropertyName("eventSubscriptions")]
		public int EventSubscriptions { get; set; }
	}
}
