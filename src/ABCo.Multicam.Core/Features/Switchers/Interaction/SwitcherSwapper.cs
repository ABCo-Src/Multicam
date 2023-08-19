using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface ISwitcherSwapper : INeedsInitialization<ISwitcherEventHandler>
    { 
        ISwitcherInteractionBuffer CurrentBuffer { get; }
        Task ChangeSwitcher(SwitcherConfig config);
        void Dispose();
    }

    public class SwitcherSwapper : ISwitcherSwapper
    {
        IServiceSource _servSource;
        ISwitcher _switcher;
        ISwitcherFactory _factory;

        public ISwitcherInteractionBuffer CurrentBuffer { get; private set; }

        public SwitcherSwapper(IServiceSource servSource, ISwitcherFactory factory)
        {
            _factory = factory;
            _servSource = servSource;

            _switcher = factory.GetSwitcher(new DummySwitcherConfig());
            CurrentBuffer = servSource.Get<ISwitcherInteractionBuffer, ISwitcher>(_switcher);
        }

        public async Task ChangeSwitcher(SwitcherConfig newConfig)
        {
            // TODO: Think about what happens if this method gets slammed with requests
            var oldBuffer = CurrentBuffer;
            _switcher = _factory.GetSwitcher(newConfig);
            CurrentBuffer = await _servSource.GetBackground<ISwitcherInteractionBuffer, ISwitcher>(_switcher);
            oldBuffer.DisposeSwitcher();
        }

        public void FinishConstruction(ISwitcherEventHandler eventHandler)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
