using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "requestType")]
	[JsonDerivedType(typeof(OBSGetSceneListResponse), "GetSceneList")]
	[JsonDerivedType(typeof(OBSGetStudioModeEnabledResponse), "GetStudioModeEnabled")]
	public class OBSResponseMessage : OBSDeserializedMessage
	{
		[JsonPropertyName("requestStatus")]
		public StatusInfo? Status { get; set; }

		[JsonPropertyName("requestId")]
		public string? RequestID { get; set; }

		[JsonPropertyName("requested")]
		public string? RequestID { get; set; }

		public class StatusInfo
		{
			[JsonPropertyName("code")]
			public int? Code { get; set; }

			[JsonPropertyName("result")]
			public bool? Result { get; set; }
		}
	}
}
