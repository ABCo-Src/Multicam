using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
	public class OBSDeserializedMessage
	{
	}

	public class OBSSerializedMessage<T>
	{
		[JsonPropertyName("op")]
		public int OpCode { get; set; }

		[JsonPropertyName("d")]
		public T Data { get; set; }

		public OBSSerializedMessage(int opCode, T data)
		{
			OpCode = opCode;
			Data = data;
		}
	}
}
