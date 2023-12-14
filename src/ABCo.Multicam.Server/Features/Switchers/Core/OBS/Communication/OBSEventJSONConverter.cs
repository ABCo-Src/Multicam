using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
    internal class OBSEventJSONConverter : JsonConverter<OBSEventMessage>
	{
		public override OBSEventMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

			string? eventType = null;
			ReadOnlySpan<byte> eventDataPos = new();

			// Collect the data involved
			while (reader.TokenType != JsonTokenType.EndObject)
			{
				if (reader.TokenType != JsonTokenType.PropertyName)
				{
					reader.Read();
					continue;
				}

				switch (OBSJSONConverterHelpers.ReadString(ref reader))
				{
					case "eventType":
						eventType = OBSJSONConverterHelpers.ReadString(ref reader);
						break;
					case "eventData":
						OBSJSONConverterHelpers.ReadAndGetSpanOfStartAndEnd(ref reader, ref eventDataPos);
						break;
				}
			}

			// If anything was missing, stop.
			if (eventType == null || eventDataPos.Length == 0) goto MissingJSONData;

			// Deserialize the specific data.
			OBSData? specificResponse = eventType switch
			{
				"SceneListChanged" => JsonSerializer.Deserialize<SceneListData>(eventDataPos, options),
				"StudioModeStateChanged" => JsonSerializer.Deserialize<StudioModeEnabledData>(eventDataPos, options),
				_ => null,
			};

			if (specificResponse == null) return null;
			return new OBSEventMessage(specificResponse);

		MissingJSONData:
			throw new OBSCommunicationException("Missing JSON property in OBS data response.");
		}

		public override void Write(Utf8JsonWriter writer, OBSEventMessage value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
