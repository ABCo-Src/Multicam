using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Interaction
{
	public interface IHotSwappableSwitcherInteractionBuffer : INeedsInitialization<SwitcherConfig>, IDisposable
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

		public HotSwappableSwitcherInteractionBuffer(IServiceSource servSource)
		{
			_servSource = servSource;
		}

		public void FinishConstruction(SwitcherConfig config) => CurrentBuffer = _servSource.Get<IPerSwitcherInteractionBuffer, SwitcherConfig>(config);

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
