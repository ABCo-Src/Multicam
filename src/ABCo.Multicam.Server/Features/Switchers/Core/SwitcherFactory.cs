using ABCo.Multicam.Server.Features.Switchers.Core.ATEM;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;

namespace ABCo.Multicam.Server.Features.Switchers.Core
{
	// Creates a switcher from a given config
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
            return config switch
            {
                DummySwitcherConfig d => _servSource.Get<IDummySwitcher, DummySwitcherConfig>(d),
                ATEMSwitcherConfig a => _servSource.Get<IATEMSwitcher, ATEMSwitcherConfig>(a),
                _ => throw new Exception("Unsupported switcher type!")
            };
        }
    }
}