using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Features.Switchers.Types;
using ABCo.Multicam.Server.Features.Switchers.Types.ATEM;

namespace ABCo.Multicam.Server.Features.Switchers
{
	// Creates a switcher from a given config
	public interface ISwitcherFactory 
    {
        ISwitcher GetSwitcher(SwitcherConfig config);
    }

    public class SwitcherFactory : ISwitcherFactory
    {
		readonly IServerInfo _servSource;
        public SwitcherFactory(IServerInfo servSource) => _servSource = servSource;

        public ISwitcher GetSwitcher(SwitcherConfig config)
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