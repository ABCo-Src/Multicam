using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Requests
{
	public class OBSGetSceneListResponse : OBSResponseMessage
	{
		[JsonPropertyName("responseData")]
		public DataClass? Data { get; set; }
		public class DataClass
		{
			// currentProgramSceneName
			// currentPreviewSceneName
			[JsonPropertyName("scenes")]
			public OBSSceneData[]? Scenes { get; set; }
		}
	}

	public class OBSSceneData
	{
		[JsonPropertyName("sceneIndex")]
		public int? SceneIndex { get; set; }

		[JsonPropertyName("sceneName")]
		public string? SceneName { get; set; }
	}
}
