namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
	public interface IHotSwappableSwitcherInteractionBuffer : IParameteredService<SwitcherConfig>, IDisposable
	{
		void SetEventHandler(ISwitcherEventHandler? handler);
		void ChangeSwitcher(SwitcherConfig config);
		IPerSwitcherInteractionBuffer CurrentBuffer { get; }
	}

	public class HotSwappableSwitcherInteractionBuffer : IHotSwappableSwitcherInteractionBuffer
	{
		IServiceSource _servSource;
		ISwitcherEventHandler? _handler;

		public IPerSwitcherInteractionBuffer CurrentBuffer { get; private set; } = null!;

		public static IHotSwappableSwitcherInteractionBuffer New(SwitcherConfig config, IServiceSource servSource) => new HotSwappableSwitcherInteractionBuffer(config, servSource);
		public HotSwappableSwitcherInteractionBuffer(SwitcherConfig config, IServiceSource servSource)
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
			_handler?.OnSpecsChange(CurrentBuffer.Specs);
			_handler?.OnConnectionStateChange(CurrentBuffer.IsConnected);
		}

		public void SetEventHandler(ISwitcherEventHandler? handler)
		{
			_handler = handler;
			CurrentBuffer.SetEventHandler(handler);
		}

		public void Dispose() => CurrentBuffer.Dispose();
	}
}
