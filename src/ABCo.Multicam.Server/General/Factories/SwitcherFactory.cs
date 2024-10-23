using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Core;
using ABCo.Multicam.Server.Features.Switchers.Core.ATEM;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS;
using ABCo.Multicam.Server.Features.Switchers.Core.Wrappers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.General.Queues;

namespace ABCo.Multicam.Server.General.Factories
{
    // Creates a raw switcher from a given config
    public interface ISwitcherFactory
    {
        ISwitcher CreateSwitcher();
        IRawSwitcher CreateRawSwitcher(SwitcherConfig config);
    }

    public class SwitcherFactory : ISwitcherFactory
    {
        readonly IServerInfo _info;
        public SwitcherFactory(IServerInfo servSource) => _info = servSource;

        public ISwitcher CreateSwitcher() => new Switcher(_info);

        public IRawSwitcher CreateRawSwitcher(SwitcherConfig config) => config switch
        {
            VirtualSwitcherConfig d => new VirtualSwitcher(d),
            ATEMSwitcherConfig a => new ATEMSwitcher(a, _info),
            OBSSwitcherConfig o => new OBSSwitcher(o, _info),
            _ => throw new Exception("Unsupported switcher type!"),
        };
    }
}