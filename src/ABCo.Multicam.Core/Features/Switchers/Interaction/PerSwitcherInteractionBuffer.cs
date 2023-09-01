using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
    public interface IPerSwitcherInteractionBuffer : INeedsInitialization<SwitcherConfig>, ISwitcherEventHandler
    {
        bool IsConnected { get; }
        SwitcherSpecs Specs { get; }
		int GetProgram(int mixBlock);
		int GetPreview(int mixBlock);
		void SendProgram(int mixBlock, int value);
		void SendPreview(int mixBlock, int value);
		void Cut(int mixBlock);
		void SetEventHandler(ISwitcherEventHandler? handler);
        void Dispose();
    }

    public class PerSwitcherInteractionBuffer : IPerSwitcherInteractionBuffer
    {
        // TODO: Handle interactions when disconnected 
        IServiceSource _servSource;
        ISwitcherFactory _factory;
		IPerSpecSwitcherInteractionBuffer _currentBuffer = null!;
		ISwitcher _switcher = null!;
        ISwitcherEventHandler? _eventHandler;

        public bool IsConnected { get; private set; }
		public SwitcherSpecs Specs => _currentBuffer.Specs;

        public PerSwitcherInteractionBuffer(IServiceSource servSource, ISwitcherFactory factory)
        {
            _factory = factory;
            _servSource = servSource;
		}

        public void FinishConstruction(SwitcherConfig config)
        {
			// Update the switcher
			_switcher = _factory.GetSwitcher(config);
			_switcher.SetEventHandler(this);

			// Request a connection status update, and use an empty buffer in the meantime
			_currentBuffer = _servSource.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(new(), _switcher);
            _switcher.RefreshConnectionStatus();
		}

		// Switcher events:
		public void OnConnectionStateChange(bool isConnected)
		{
			IsConnected = isConnected;
			if (isConnected) _switcher.RefreshSpecs();
			_eventHandler?.OnConnectionStateChange(isConnected);
		}

		public void OnSpecsChange(SwitcherSpecs newSpecs)
		{
			_currentBuffer = _servSource.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(newSpecs, _switcher);
			_currentBuffer.SetEventHandler(_eventHandler);

			// If we're connected, update the values too
			if (IsConnected) _currentBuffer.UpdateEverything();

			_eventHandler?.OnSpecsChange(newSpecs);
		}

		public void OnProgramValueChange(SwitcherProgramChangeInfo info)
		{
            _currentBuffer.UpdateProg(info);
			_eventHandler?.OnProgramValueChange(info);
		}

		public void UpdatePreview(SwitcherPreviewChangeInfo info)
		{
			_currentBuffer.UpdatePrev(info);
			_eventHandler?.UpdatePreview(info);
		}

		public void SetEventHandler(ISwitcherEventHandler? eventHandler)
		{
			_currentBuffer.SetEventHandler(eventHandler);
			_eventHandler = eventHandler;
		}

		// Actions:
		public int GetProgram(int mixBlock) => _currentBuffer.GetProgram(mixBlock);
		public int GetPreview(int mixBlock) => _currentBuffer.GetPreview(mixBlock);
		public void SendProgram(int mixBlock, int value) => _currentBuffer.SendProgram(mixBlock, value);
		public void SendPreview(int mixBlock, int value) => _currentBuffer.SendPreview(mixBlock, value);
		public void Cut(int mixBlock) => _currentBuffer.Cut(mixBlock);

		public void Dispose() => _switcher.Dispose();
    }
}
