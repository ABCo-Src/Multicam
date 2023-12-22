using ABCo.Multicam.Server.Features.Switchers.Core.ATEM;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS;
using ABCo.Multicam.Server.Features.Switchers.Core.Wrappers;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.General.Queues;

namespace ABCo.Multicam.Server.Features.Switchers.Core
{
	// Creates a raw switcher from a given config
	public interface ISwitcherFactory
    {
        IRawSwitcher GetSwitcher(SwitcherConfig config);
    }

    public class SwitcherFactory : ISwitcherFactory
    {
        readonly IServerInfo _servSource;
        public SwitcherFactory(IServerInfo servSource) => _servSource = servSource;

        public IRawSwitcher GetSwitcher(SwitcherConfig config)
        {
            switch (config)
            {
                case VirtualSwitcherConfig d:
                    return _servSource.Get<IVirtualSwitcher, VirtualSwitcherConfig>(d);

                case ATEMSwitcherConfig a:
                    var atem = _servSource.Get<IATEMSwitcher, ATEMSwitcherConfig>(a);
                    var caughtATEM = new CatchingSwitcherWrapper(atem);
					return new ExecutionBufferSwitcherWrapper(caughtATEM, new BackgroundThreadExecutionBuffer<IRawSwitcher>(true, caughtATEM));

                case OBSSwitcherConfig o:
					var obs = new OBSSwitcher(o);
                    var caughtOBS = new CatchingSwitcherWrapper(obs);
                    return new ExecutionBufferSwitcherWrapper(caughtOBS, new SameThreadExecutionBuffer<IRawSwitcher>(caughtOBS));

                default:
                    throw new Exception("Unsupported switcher type!");
			}
        }
    }
}