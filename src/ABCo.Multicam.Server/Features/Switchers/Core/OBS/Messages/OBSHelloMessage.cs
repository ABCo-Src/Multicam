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
		public Authentication? Auth { get; set; }

		[JsonPropertyName("obsWebSocketVersion"), JsonRequired]
		public string OBSWebSocketVersion { get; set; } = null!;

		[JsonPropertyName("rpcVersion"), JsonRequired]
		public int RPCVersion { get; set; }

		public class Authentication
		{
			[JsonPropertyName("challenge"), JsonRequired]
			public string Challenge { get; set; } = null!;

			[JsonPropertyName("salt"), JsonRequired]
			public string Salt { get; set; } = null!;
		}
	}
}
