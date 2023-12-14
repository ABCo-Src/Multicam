using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data
{
	public class CurrentPreviewSceneData : OBSData
	{
		public string SceneName { get; set; } = null!;
		public CurrentPreviewSceneData(string sceneName) => SceneName = sceneName;
	}
}
