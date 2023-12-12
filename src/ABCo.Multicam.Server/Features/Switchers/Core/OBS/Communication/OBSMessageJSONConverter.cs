using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
	public class OBSMessageJSONConverter : JsonConverter<OBSDeserializedMessage>
	{
		readonly OBSReceiveBuffer _currentlyDeserializing;

		public OBSMessageJSONConverter(OBSReceiveBuffer currentlyDeserializing) => _currentlyDeserializing = currentlyDeserializing;

		public override OBSDeserializedMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

			int? opCode = null;
			ReadOnlySpan<byte> dataPos = new();

			// Find the opcode and what position the data starts at.
			while (reader.TokenType != JsonTokenType.EndObject)
			{
				if (reader.TokenType != JsonTokenType.PropertyName)
				{
					reader.Read();
					continue;
				}

				switch (reader.GetString())
				{
					case "op":
						// Read property name, get value, read value
						reader.Read();
						opCode = reader.GetInt32();
						reader.Read();
						break;

					case "d":
						// Read property name, save start of data, skip data, save end of data, read EndObject
						reader.Read();
						dataStart = reader.TokenStartIndex;
						reader.Skip();
						dataEnd = reader.TokenStartIndex;
						reader.Read();
						break;
					default:
						throw new Exception("Unexpected JSON property in OBS data");
				}
			}

			checked
			{
				// If anything was missing, stop.
				if (opCode == null || dataStart == null) throw new Exception("Missing JSON property in OBS message");
				if (dataStart > int.MaxValue || dataEnd > int.MaxValue) throw new Exception("Infinite data received from OBS");

				// Deserialize the data with the appropriate type
				var dataSpan = _currentlyDeserializing.Buffer.AsSpan()[(int)dataStart.Value..(int)(dataEnd + 1)];
				return opCode switch
				{
					0 => JsonSerializer.Deserialize<OBSHelloMessage>(dataSpan),
					2 => JsonSerializer.Deserialize<OBSIdentifiedMessage>(dataSpan),
					7 => JsonSerializer.Deserialize<OBSResponseMessage>(dataSpan),
					_ => throw new Exception("Unsupported OBS message opcode"),
				};
			}

			string? ReadString(ref Utf8JsonReader reader)
			{
				string? str = reader.GetString();
				reader.Read();
				return str;
			}
		}

		public override void Write(Utf8JsonWriter writer, OBSDeserializedMessage value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
