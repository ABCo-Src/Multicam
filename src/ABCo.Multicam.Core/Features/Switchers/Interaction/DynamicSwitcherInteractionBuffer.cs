using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IDynamicSwitcherInteractionBuffer : INeedsInitialization<ISwitcherEventHandler>, ISwitcherEventHandler
    { 
        IPerSpecSwitcherInteractionBuffer CurrentBuffer { get; }
        void ChangeSwitcher(SwitcherConfig config);
        void Dispose();
    }

    public class DynamicSwitcherInteractionBuffer : IDynamicSwitcherInteractionBuffer
    {
        IServiceSource _servSource;
        ISwitcherFactory _factory;
        ISwitcher _switcher;
        ISwitcherEventHandler _eventHandler = null!;

        public IPerSpecSwitcherInteractionBuffer CurrentBuffer { get; private set; }

        public DynamicSwitcherInteractionBuffer(IServiceSource servSource, ISwitcherFactory factory)
        {
            _factory = factory;
            _servSource = servSource;

            _switcher = factory.GetSwitcher(new DummySwitcherConfig());
            CurrentBuffer = servSource.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(new(), _switcher);
			CurrentBuffer.SetEventHandler(this);
		}

        public void ChangeSwitcher(SwitcherConfig newConfig)
        {
            _switcher = _factory.GetSwitcher(newConfig);

            CurrentBuffer.DisposeSwitcher();
            CurrentBuffer.SetEventHandler(null);
            CurrentBuffer = _servSource.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(new(), _switcher);
			CurrentBuffer.SetEventHandler(this);

			if (_switcher.IsConnected) _switcher.RefreshSpecs();
        }

        public void FinishConstruction(ISwitcherEventHandler eventHandler) => _eventHandler = eventHandler;

        public void OnProgramChangeFinish(SwitcherProgramChangeInfo info) => _eventHandler.OnProgramChangeFinish(info);
        public void OnPreviewChangeFinish(SwitcherPreviewChangeInfo info) => _eventHandler.OnPreviewChangeFinish(info);
        public void OnSpecsChange(SwitcherSpecs newSpecs) 
        {
            CurrentBuffer = _servSource.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(newSpecs, _switcher);
			CurrentBuffer.SetEventHandler(this);
			_eventHandler.OnSpecsChange(newSpecs);
        }

        public void Dispose() => CurrentBuffer.DisposeSwitcher();
    }
}
