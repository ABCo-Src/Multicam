using ABCo.Multicam.Server.Features.Switchers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.Wrappers
{
	/// <summary>
	/// A base class for a wrapping switcher that passes every action through to the "next", and every event through to the assigned handler.
	/// Methods are overridable for custom functionality.
	/// </summary>
	public class PassthroughSwitcherBase : IRawSwitcher, ISwitcherEventHandler
	{
		protected readonly IRawSwitcher _nextSwitcher;
		protected ISwitcherEventHandler? _parentSwitcher;

		public PassthroughSwitcherBase(IRawSwitcher nextSwitcher)
		{
			_nextSwitcher = nextSwitcher;
			nextSwitcher.SetEventHandler(this);
		}

		public virtual void Connect() => _nextSwitcher.Connect();
		public virtual void Disconnect() => _nextSwitcher.Disconnect();
		public virtual void Cut(int mixBlock) => _nextSwitcher.Cut(mixBlock);
		public virtual SwitcherPlatformCompatibilityValue GetPlatformCompatibility() => _nextSwitcher.GetPlatformCompatibility();
		public virtual void RefreshConnectionStatus() => _nextSwitcher.RefreshConnectionStatus();
		public virtual void RefreshPreview(int mixBlock) => _nextSwitcher.RefreshPreview(mixBlock);
		public virtual void RefreshProgram(int mixBlock) => _nextSwitcher.RefreshProgram(mixBlock);
		public virtual void RefreshSpecs() => _nextSwitcher.RefreshSpecs();
		public virtual void SendPreviewValue(int mixBlock, int id) => _nextSwitcher.SendPreviewValue(mixBlock, id);
		public virtual void SendProgramValue(int mixBlock, int id) => _nextSwitcher.SendProgramValue(mixBlock, id);
		public virtual void SetEventHandler(ISwitcherEventHandler? eventHandler) => _parentSwitcher = eventHandler;
		public virtual void Dispose() => _nextSwitcher.Dispose();
		public virtual void OnProgramValueChange(SwitcherProgramChangeInfo info) => _parentSwitcher?.OnProgramValueChange(info);
		public virtual void OnPreviewValueChange(SwitcherPreviewChangeInfo info) => _parentSwitcher?.OnPreviewValueChange(info);
		public virtual void OnSpecsChange(SwitcherSpecs newSpecs) => _parentSwitcher?.OnSpecsChange(newSpecs);
		public virtual void OnConnectionStateChange(bool isConnected) => _parentSwitcher?.OnConnectionStateChange(isConnected);
		public virtual void OnFailure(SwitcherError error) => _parentSwitcher?.OnFailure(error);
	}
}
