using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.General;

namespace ABCo.Multicam.Core.Features.Switchers.Types.ATEM
{
	public interface IATEMSwitcher : ISwitcher, IErrorHandlingTarget, IParameteredService<ATEMSwitcherConfig>
	{
	}

	public class ATEMSwitcherConfig : SwitcherConfig 
	{
		public override SwitcherType Type => SwitcherType.ATEM;
	}

	public class ATEMSwitcher : Switcher, IATEMSwitcher
	{
		readonly IMainThreadDispatcher _mainThreadDispatcher;
		readonly IServiceSource _servSource;
		readonly CatchingAndQueuedSTAThread<ATEMSwitcher> _interactionThread = new();

		IATEMConnection? _connection; // MUST always be used from the background queue

		public ATEMSwitcher(ATEMSwitcherConfig config, IServiceSource servSource)
		{
			_servSource = servSource;
			_mainThreadDispatcher = servSource.Get<IMainThreadDispatcher>();
		}

		public override void Connect()
		{
			_interactionThread.QueueTask(s =>
			{
				s._connection = s._servSource.Get<IATEMConnection, IATEMSwitcher>(s);
				s._mainThreadDispatcher.QueueOnMainFeatureThread(() => s._eventHandler?.OnConnectionStateChange(true));
			}, this);

			_interactionThread.StartExecution();
		}

		public override void Disconnect()
		{
			_interactionThread.QueueTask(s =>
			{
				if (s._connection == null) throw new UnexpectedSwitcherDisconnectionException();

				s._connection.Dispose();
				s._connection = null;
				s._mainThreadDispatcher.QueueOnMainFeatureThread(() => s._eventHandler?.OnConnectionStateChange(false));
			}, this);

			_interactionThread.QueueFinish();
		}

		public override void RefreshConnectionStatus() => _eventHandler?.OnConnectionStateChange(_connection != null);

		public override void RefreshSpecs()
		{
			_interactionThread.QueueTask(s =>
			{
				if (s._connection == null) throw new UnexpectedSwitcherDisconnectionException();

				var newSpecs = s._connection.InvalidateCurrentSpecs();
				s._mainThreadDispatcher.QueueOnMainFeatureThread(() => s._eventHandler?.OnSpecsChange(newSpecs));
			}, this);
		}

		public override void RefreshProgram(int mixBlock)
		{
			_interactionThread.QueueTask(s =>
			{
				if (s._connection == null) throw new UnexpectedSwitcherDisconnectionException();

				long val = s._connection.GetProgram(mixBlock);
				s._mainThreadDispatcher.QueueOnMainFeatureThread(() => _eventHandler?.OnProgramValueChange(new SwitcherProgramChangeInfo(mixBlock, (int)val, null)));
			}, this);
		}

		public override void RefreshPreview(int mixBlock)
		{
			_interactionThread.QueueTask(s =>
			{
				if (s._connection == null) throw new UnexpectedSwitcherDisconnectionException();

				long val = s._connection.GetPreview(mixBlock);
				_mainThreadDispatcher.QueueOnMainFeatureThread(() => _eventHandler?.OnPreviewValueChange(new SwitcherPreviewChangeInfo(mixBlock, (int)val, null)));
			}, this);
		}

		public override void SendProgramValue(int mixBlock, int id)
		{
			_interactionThread.QueueTask(s =>
			{
				if (s._connection == null) throw new UnexpectedSwitcherDisconnectionException();
				s._connection.SendProgram(mixBlock, id);
			}, this);
		}

		public override void SendPreviewValue(int mixBlock, int id)
		{
			_interactionThread.QueueTask(s =>
			{
				if (s._connection == null) throw new UnexpectedSwitcherDisconnectionException();
				s._connection.SendPreview(mixBlock, id);
			}, this);
		}

		public override void Cut(int mixBlock)
		{
			_interactionThread.QueueTask(s =>
			{
				if (s._connection == null) throw new UnexpectedSwitcherDisconnectionException();
				s._connection.Cut(mixBlock);
			}, this);
		}

		public void OnATEMDisconnect() => _mainThreadDispatcher.QueueOnMainFeatureThread(() => _eventHandler?.OnConnectionStateChange(false));
		public void OnATEMProgramChange(int mixBlock) => RefreshProgram(mixBlock);
		public void OnATEMPreviewChange(int mixBlock) => RefreshPreview(mixBlock);

		public void ProcessError(Exception ex) => _mainThreadDispatcher.QueueOnMainFeatureThread(() => _eventHandler?.OnFailure(new(ex.Message)));

		public override void Dispose() 
		{
			_interactionThread.QueueTask(s => s._connection?.Dispose(), this);
			_interactionThread.QueueFinish();
		}

		public record struct RawInputData(long Id, string Name, byte MixBlockMask);
	}
}