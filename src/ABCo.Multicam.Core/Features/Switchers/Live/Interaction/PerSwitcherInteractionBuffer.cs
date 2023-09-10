using ABCo.Multicam.Core.Features.Switchers.Data;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
	public interface IPerSwitcherInteractionBuffer : IParameteredService<SwitcherConfig>, ISwitcherEventHandler
    {
        bool IsConnected { get; }
        SwitcherSpecs Specs { get; }
		void Connect();
		void Disconnect();
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
		readonly IServiceSource _servSource;
		readonly ISwitcherFactory _factory;
		IPerSpecSwitcherInteractionBuffer _currentBuffer = null!;
		readonly ISwitcher _switcher = null!;
        ISwitcherEventHandler? _eventHandler;

        public bool IsConnected { get; private set; }
		public SwitcherSpecs Specs => _currentBuffer.Specs;

		public static IPerSwitcherInteractionBuffer New(SwitcherConfig config, IServiceSource servSource) => new PerSwitcherInteractionBuffer(config, servSource);
        public PerSwitcherInteractionBuffer(SwitcherConfig config, IServiceSource servSource)
        {
            _factory = servSource.Get<ISwitcherFactory>();
            _servSource = servSource;

			// Update the switcher
			_switcher = _factory.GetSwitcher(config);
			_switcher.SetEventHandler(this);

			// Request a connection status update, and use an empty buffer in the meantime
			_currentBuffer = _servSource.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(new(true), _switcher);
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

		public void OnPreviewValueChange(SwitcherPreviewChangeInfo info)
		{
			_currentBuffer.UpdatePrev(info);
			_eventHandler?.OnPreviewValueChange(info);
		}

		public void OnFailure(SwitcherError error)
		{
			_eventHandler?.OnFailure(error);

			_switcher.SetEventHandler(null); // Detach the switcher so if things are really bad the Dispose doesn't make an infinite error loop with us
			_switcher.Dispose();
		}

		public void SetEventHandler(ISwitcherEventHandler? eventHandler)
		{
			_currentBuffer.SetEventHandler(eventHandler);
			_eventHandler = eventHandler;
		}

		// Actions:
		public void Connect() 
		{
			if (!_currentBuffer.Specs.CanChangeConnection) return;
			_switcher.Connect();
		}

		public void Disconnect() 
		{
			if (!_currentBuffer.Specs.CanChangeConnection) return;
			_switcher.Disconnect();
		}

		public int GetProgram(int mixBlock) => _currentBuffer.GetProgram(mixBlock);
		public int GetPreview(int mixBlock) => _currentBuffer.GetPreview(mixBlock);
		public void SendProgram(int mixBlock, int value) => _currentBuffer.SendProgram(mixBlock, value);
		public void SendPreview(int mixBlock, int value) => _currentBuffer.SendPreview(mixBlock, value);
		public void Cut(int mixBlock) => _currentBuffer.Cut(mixBlock);

		public void Dispose() => _switcher.Dispose();
    }
}
