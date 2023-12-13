using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.NewData
{
    public class OBSGetSceneListResponse : OBSResponseMessage
    {
        [JsonPropertyName("scenes"), JsonRequired]
        public OBSSceneData[] Scenes { get; set; } = null!;
    }

    public class OBSSceneData
    {
        [JsonPropertyName("sceneIndex"), JsonRequired]
        public int SceneIndex { get; set; }

        [JsonPropertyName("sceneName"), JsonRequired]
        public string SceneName { get; set; } = null!;
    }
}
