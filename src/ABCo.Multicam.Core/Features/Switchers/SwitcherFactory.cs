using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    // Creates a switcher from a given config
    public interface ISwitcherFactory 
    {
        ISwitcher GetSwitcher(SwitcherConfig config);
    }

    public class SwitcherFactory : ISwitcherFactory
    {
        IServiceSource _servSource;
        public SwitcherFactory(IServiceSource servSource) => _servSource = servSource;

        public ISwitcher GetSwitcher(SwitcherConfig config)
        {
            return config switch
            {
                DummySwitcherConfig d => _servSource.Get<IDummySwitcher, DummySwitcherConfig>(d),
                _ => throw new Exception("Unsupported switcher type!")
            };
        }
    }
}