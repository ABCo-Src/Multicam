using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
	public class OBSResponseMessage : OBSDeserializedMessage
	{
		public OBSResponseStatus Status { get; set; } = null!;
	}

	public class OBSResponseStatus
	{
		[JsonPropertyName("code"), JsonRequired]
		public int Code { get; set; }

		[JsonPropertyName("result"), JsonRequired]
		public bool Result { get; set; }
	}
}
