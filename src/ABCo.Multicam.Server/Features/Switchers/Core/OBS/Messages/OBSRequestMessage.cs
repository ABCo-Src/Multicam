using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
	public class OBSRequestMessage<T> : OBSRequestMessage
	{
		[JsonPropertyName("requestData")]
		public T RequestData { get; set; }

		public OBSRequestMessage(string requestType, string requestID, T requestData) : base(requestType, requestID) => RequestData = requestData;
	}

	public class OBSRequestMessage : OBSDeserializedMessage
	{
		[JsonPropertyName("requestType")]
		public string RequestType { get; set; }

		[JsonPropertyName("requestId")]
		public string RequestID { get; set; }

		public OBSRequestMessage(string requestType, string requestID)
		{
			RequestType = requestType;
			RequestID = requestID;
		}
	}
}
