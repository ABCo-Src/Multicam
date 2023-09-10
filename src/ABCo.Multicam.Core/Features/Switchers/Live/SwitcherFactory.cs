using ABCo.Multicam.Core.Features.Switchers.Data.Config;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;

namespace ABCo.Multicam.Core.Features.Switchers
{
	// Creates a switcher from a given config
	public interface ISwitcherFactory 
    {
        ISwitcher GetSwitcher(SwitcherConfig config);
    }

    public class SwitcherFactory : ISwitcherFactory
    {
		readonly IServiceSource _servSource;
        public SwitcherFactory(IServiceSource servSource) => _servSource = servSource;

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