using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
	public class OBSHelloMessage : OBSDeserializedMessage
	{
		[JsonPropertyName("authentication")]
		public Authentication? Auth { get; set; } = null;

		[JsonPropertyName("obsWebSocketVersion")]
		public string? OBSWebSocketVersion { get; set; }

		[JsonPropertyName("rpcVersion")]
		public int? RPCVersion { get; set; }

		public class Authentication
		{
			[JsonPropertyName("challenge")]
			public string? Challenge { get; set; }

			[JsonPropertyName("salt")]
			public string? Salt { get; set; }
		}
	}
}
