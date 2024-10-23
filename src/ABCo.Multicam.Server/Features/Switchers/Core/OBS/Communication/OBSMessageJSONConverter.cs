using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
    public class OBSMessageJSONConverter : JsonConverter<OBSDeserializedMessage>
    {
        public OBSMessageJSONConverter() { }

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

                switch (OBSJSONConverterHelpers.ReadString(ref reader))
                {
                    case "op":
                        opCode = reader.GetInt32();
                        reader.Read();
                        break;
                    case "d":
                        OBSJSONConverterHelpers.ReadAndGetSpanOfStartAndEnd(ref reader, ref dataPos);
                        break;
                }
            }

            // If anything was missing, stop.
            if (opCode == null || dataPos.Length == 0) throw new OBSCommunicationException("Missing JSON property in OBS message.");

            // Deserialize the data with the appropriate type
            return opCode switch
            {
                0 => JsonSerializer.Deserialize<OBSHelloMessage>(dataPos, options),
                2 => JsonSerializer.Deserialize<OBSIdentifiedMessage>(dataPos, options),
                5 => JsonSerializer.Deserialize<OBSEventMessage>(dataPos, options),
                7 => JsonSerializer.Deserialize<OBSResponseMessage>(dataPos, options),
                _ => throw new OBSCommunicationException("Unsupported OBS message opcode."),
            };
        }

        public override void Write(Utf8JsonWriter writer, OBSDeserializedMessage value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
