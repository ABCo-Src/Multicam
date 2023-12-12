using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Requests;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
	public class OBSResponseJSONConverter : JsonConverter<OBSResponse>
	{
		public override OBSResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

			string? requestType = null;
			ReadOnlySpan<byte> requestStatusPos = new();
			ReadOnlySpan<byte> responseDataPos = new();

			// Collect the data involved
			while (reader.TokenType != JsonTokenType.EndObject)
			{
				if (reader.TokenType != JsonTokenType.PropertyName)
				{
					reader.Read();
					continue;
				}

				switch (ReadString(ref reader))
				{
					case "requestType":
						requestType = ReadString(ref reader);
						break;
					case "requestStatus":
						reader.Skip();
						requestStatusPos = reader.ValueSpan;
						break;
					case "responseData":
						reader.Skip();
						responseDataPos = reader.ValueSpan;
						break;
						//default:
						//	throw new Exception("Unexpected JSON property in OBS data");
				}
			}

			// If anything was missing, stop.
			if (requestType == null || requestStatusPos.Length == 0 || responseDataPos.Length == 0) throw new OBSCommunicationException("Missing JSON property in OBS data response.");

			// Deserialize the data with the appropriate type
			return requestType switch
			{
				0 => JsonSerializer.Deserialize<OBSHelloMessage>(dataSpan),
				2 => JsonSerializer.Deserialize<OBSIdentifiedMessage>(dataSpan),
				7 => JsonSerializer.Deserialize<OBSResponseMessage>(dataSpan),
				_ => throw new Exception("Unsupported OBS message opcode"),
			};

			string? ReadString(ref Utf8JsonReader reader)
			{
				string? str = reader.GetString();
				reader.Read();
				return str;
			}
		}

		Range GetRangeOfToken(ref Utf8JsonReader reader)
		{
			reader.Read();
			int dataStart = checked((int)reader.TokenStartIndex);
			reader.Skip();
			int dataEnd = reader.TokenStartIndex;
			reader.Read();

			return new Range(new Index(dataStart), new Index(dataEnd));
		}

		public override void Write(Utf8JsonWriter writer, OBSResponse value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
