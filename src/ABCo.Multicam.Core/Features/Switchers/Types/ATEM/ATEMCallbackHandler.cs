using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Types.ATEM
{
    public interface IATEMCallbackHandler : INeedsInitialization<IATEMConnectionEventHandler>
    {
        void AttachToSwitcher(IBMDSwitcher switcher);
		void DetachFromSwitcher(IBMDSwitcher switcher);
        void AttachMixBlocks(IBMDSwitcherMixEffectBlock[] mixBlocks);
		void DetachMixBlocks(IBMDSwitcherMixEffectBlock[] mixBlocks);
	}

	internal class ATEMCallbackHandler : IATEMCallbackHandler, IBMDSwitcherCallback
	{
		IATEMConnectionEventHandler _handler = null!;
		MixEffectBlockHandler[] _handlers = Array.Empty<MixEffectBlockHandler>();

		public void FinishConstruction(IATEMConnectionEventHandler handler) => _handler = handler;

		public void Notify(_BMDSwitcherEventType type, _BMDSwitcherVideoMode videoMode)
		{
			if (type == _BMDSwitcherEventType.bmdSwitcherEventTypeDisconnected)
				_handler.OnATEMDisconnect();
		}

		public void AttachToSwitcher(IBMDSwitcher switcher) => switcher.AddCallback(this);
		public void DetachFromSwitcher(IBMDSwitcher switcher) => switcher.RemoveCallback(this);

		public void DetachMixBlocks(IBMDSwitcherMixEffectBlock[] mixBlocks)
		{
			for (int i = 0; i < mixBlocks.Length; i++)
				mixBlocks[i].RemoveCallback(_handlers[i]);

			_handlers = Array.Empty<MixEffectBlockHandler>();
		}

		public void AttachMixBlocks(IBMDSwitcherMixEffectBlock[] mixBlocks)
		{
			Debug.Assert(_handlers.Length == 0); // Old mix-blocks should have been detached

			_handlers = new MixEffectBlockHandler[mixBlocks.Length];
			for (int i = 0; i < mixBlocks.Length; i++)
			{
				_handlers[i] = new MixEffectBlockHandler(_handler, i);
				mixBlocks[i].AddCallback(_handlers[i]);
			}
		}

		class MixEffectBlockHandler : IBMDSwitcherMixEffectBlockCallback
        {
			IATEMConnectionEventHandler _handler;
			int _index;

			public MixEffectBlockHandler(IATEMConnectionEventHandler handler, int index) => (_handler, _index) = (handler, index);

			public void Notify(_BMDSwitcherMixEffectBlockEventType eventType)
			{
				if (eventType == _BMDSwitcherMixEffectBlockEventType.bmdSwitcherMixEffectBlockEventTypeProgramInputChanged)
					_handler.OnATEMProgramChange(_index);
				else if (eventType == _BMDSwitcherMixEffectBlockEventType.bmdSwitcherMixEffectBlockEventTypePreviewInputChanged)
					_handler.OnATEMPreviewChange(_index);
			}
		}
    }
}
