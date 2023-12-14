using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data
{
    public class SceneListData : OBSData
    {
        [JsonPropertyName("scenes"), JsonRequired]
        public SceneData[] Scenes { get; set; } = null!;
    }

    public class SceneData
    {
        [JsonPropertyName("sceneIndex"), JsonRequired]
        public int SceneIndex { get; set; }

        [JsonPropertyName("sceneName"), JsonRequired]
        public string SceneName { get; set; } = null!;
    }
}
