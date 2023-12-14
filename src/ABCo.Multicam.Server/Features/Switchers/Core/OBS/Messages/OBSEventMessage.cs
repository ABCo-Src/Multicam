using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages
{
    public class OBSEventMessage : OBSDeserializedMessage
    {
		public OBSData Data { get; set; } = null!;
		public OBSEventMessage(OBSData data) => Data = data;
	}
}
