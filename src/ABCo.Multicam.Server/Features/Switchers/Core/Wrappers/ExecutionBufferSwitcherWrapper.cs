using ABCo.Multicam.Server.General.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.Wrappers
{
    /// <summary>
    /// Puts an execution buffer between all of the switcher's operations.
    /// </summary>
    public class ExecutionBufferSwitcherWrapper : PassthroughSwitcherBase
	{
		readonly IExecutionBuffer<IRawSwitcher> _interactionThread;

		public ExecutionBufferSwitcherWrapper(IRawSwitcher nextSwitcher, IExecutionBuffer<IRawSwitcher> executionBuffer) : base(nextSwitcher)
		{
			_interactionThread = executionBuffer;
		}

		public override void Connect()
		{
			_interactionThread.QueueTask(s => s.Connect());
			_interactionThread.StartExecution();
		}

		public override void Disconnect()
		{
			_interactionThread.QueueTask(s => s.Disconnect());
			_interactionThread.QueueFinish();
		}

		public override void Cut(int mixBlock) => _interactionThread.QueueTask(s => s.Cut(mixBlock));
		public override void RefreshConnectionStatus() => _interactionThread.QueueTask(s => s.RefreshConnectionStatus());
		public override void RefreshPreview(int mixBlock) => _interactionThread.QueueTask(s => s.RefreshPreview(mixBlock));
		public override void RefreshProgram(int mixBlock) => _interactionThread.QueueTask(s => s.RefreshProgram(mixBlock));
		public override void RefreshSpecs() => _interactionThread.QueueTask(s => s.RefreshSpecs());
		public override void SendPreviewValue(int mixBlock, int id) => _interactionThread.QueueTask(s => s.SendPreviewValue(mixBlock, id));
		public override void SendProgramValue(int mixBlock, int id) => _interactionThread.QueueTask(s => s.SendProgramValue(mixBlock, id));
		public override void Dispose() => _interactionThread.QueueFinish(s => s.Dispose());
	}
}
