using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Requests
{
	public class OBSGetStudioModeEnabledResponse : OBSResponseMessage
	{
		[JsonPropertyName("responseData")]
		public DataClass? Data { get; set; }
		public class DataClass
		{
			// currentProgramSceneName
			// currentPreviewSceneName
			[JsonPropertyName("studioModeEnabled")]
			public bool? IsEnabled { get; set; }
		}
	}
}
