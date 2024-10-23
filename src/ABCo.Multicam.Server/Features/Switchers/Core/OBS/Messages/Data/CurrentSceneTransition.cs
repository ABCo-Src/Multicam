using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data
{
    public class CurrentSceneTransition : OBSData
    {
        [JsonPropertyName("transitionName")] // Used for serialization
        public string TransitionName { get; set; }
        public CurrentSceneTransition(string transitionName) => TransitionName = transitionName;
    }
}
