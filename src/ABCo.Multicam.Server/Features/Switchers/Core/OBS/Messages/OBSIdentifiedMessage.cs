using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.OBSHelloMessage;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
    public class OBSIdentifiedMessage : OBSDeserializedMessage
    {
        [JsonPropertyName("negotiatedRpcVersion"), JsonRequired]
        public int NegotiatedRPCVersion { get; set; }
    }
}
