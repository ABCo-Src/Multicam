using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.NewData;
using System.Data;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
    public class OBSResponseJSONConverter : JsonConverter<OBSResponseMessage>
	{
		public override OBSResponseMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
						OBSJSONConverterHelpers.ReadAndGetSpanOfStartAndEnd(ref reader, ref requestStatusPos);
						break;
					case "responseData":
						OBSJSONConverterHelpers.ReadAndGetSpanOfStartAndEnd(ref reader, ref responseDataPos);
						break;
				}
			}

			// If anything was missing, stop.
			if (requestType == null || requestStatusPos.Length == 0 || responseDataPos.Length == 0) goto MissingJSONData;

			// Deserialize the specific data.
			OBSResponseMessage? specificResponse = requestType switch
			{
				"GetSceneList" => JsonSerializer.Deserialize<OBSGetSceneListResponse>(responseDataPos, options),
				"GetStudioModeEnabled" => JsonSerializer.Deserialize<OBSGetStudioModeEnabledResponse>(responseDataPos, options),
				_ => null,
			};
			if (specificResponse == null) return null;

			// Deserialize status
			var status = JsonSerializer.Deserialize<OBSResponseStatus>(requestStatusPos);
			if (status == null) goto MissingJSONData;
			specificResponse.Status = status;

			// Return this
			return specificResponse;

			string? ReadString(ref Utf8JsonReader reader)
			{
				string? str = reader.GetString();
				reader.Read();
				return str;
			}

		MissingJSONData:
			throw new OBSCommunicationException("Missing JSON property in OBS data response.");
		}

		public override void Write(Utf8JsonWriter writer, OBSResponseMessage value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
