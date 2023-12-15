using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data
{
    public class StudioModeEnabledData : OBSData
    {
        [JsonPropertyName("studioModeEnabled"), JsonRequired]
        public bool IsEnabled { get; set; }

        public StudioModeEnabledData() { }
        public StudioModeEnabledData(bool isEnabled) => IsEnabled = isEnabled;
    }
}
