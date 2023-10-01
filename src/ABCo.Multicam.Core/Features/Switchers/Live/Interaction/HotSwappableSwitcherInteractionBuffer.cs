namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
	public interface IHotSwappableSwitcherInteractionBuffer : IServerService<SwitcherConfig>, IDisposable
	{
		void SetEventHandler(ISwitcherEventHandler? handler);
		void ChangeSwitcher(SwitcherConfig config);
		IPerSwitcherInteractionBuffer CurrentBuffer { get; }
	}

	public class HotSwappableSwitcherInteractionBuffer : IHotSwappableSwitcherInteractionBuffer
	{
		readonly IServerInfo _servSource;
		ISwitcherEventHandler? _handler;

		public IPerSwitcherInteractionBuffer CurrentBuffer { get; private set; } = null!;

		public HotSwappableSwitcherInteractionBuffer(SwitcherConfig config, IServerInfo servSource)
		{
			_servSource = servSource;
			CurrentBuffer = _servSource.Get<IPerSwitcherInteractionBuffer, SwitcherConfig>(config);
		}

		public void ChangeSwitcher(SwitcherConfig config)
		{
			CurrentBuffer.Dispose();
			CurrentBuffer = _servSource.Get<IPerSwitcherInteractionBuffer, SwitcherConfig>(config);
			CurrentBuffer.SetEventHandler(_handler);

			// Refresh everything to match the change
			_handler?.OnConnectionStateChange(CurrentBuffer.IsConnected);
			_handler?.OnSpecsChange(CurrentBuffer.Specs);
		}

		public void SetEventHandler(ISwitcherEventHandler? handler)
		{
			_handler = handler;
			CurrentBuffer.SetEventHandler(handler);
		}

		public void Dispose() => CurrentBuffer.Dispose();
	}
}
