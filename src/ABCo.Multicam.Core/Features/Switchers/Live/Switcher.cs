using ABCo.Multicam.Server.Features.Switchers.Data;

namespace ABCo.Multicam.Core.Features.Switchers
{
	public class UnsupportedSwitcherBehaviourException : Exception
    {
        public UnsupportedSwitcherBehaviourException() : base("Switcher was asked to perform something it can't. Either switcher is reporting incorrect specs or something higher-up went wrong.") { }
    }

	/// <summary>
	/// Provides a base class to make implementing switchers easier that:
	/// 1. Fails everything on the ISwitcher API unless overriden.
	/// 2. Stores the assigned event handler in a protected field
	/// </summary>
	public abstract class Switcher : ISwitcher
    {
        protected ISwitcherEventHandler? _eventHandler;

        public virtual void Connect() => throw new UnsupportedSwitcherBehaviourException();
        public virtual void Disconnect() => throw new UnsupportedSwitcherBehaviourException();
        public virtual void Cut(int mixBlock) => throw new UnsupportedSwitcherBehaviourException();
        public virtual CutBusMode GetCutBusMode(int mixBlock) => throw new UnsupportedSwitcherBehaviourException();
        public virtual void RefreshPreview(int mixBlock) => throw new UnsupportedSwitcherBehaviourException();
        public virtual void RefreshProgram(int mixBlock) => throw new UnsupportedSwitcherBehaviourException();
        public virtual void SendPreviewValue(int mixBlock, int id) => throw new UnsupportedSwitcherBehaviourException();
        public virtual void SendProgramValue(int mixBlock, int id) => throw new UnsupportedSwitcherBehaviourException();
        public virtual void SetCutBus(int mixBlock, int newVal) => throw new UnsupportedSwitcherBehaviourException();
        public virtual void SetCutBusMode(int mixBlock, CutBusMode mode) => throw new UnsupportedSwitcherBehaviourException();

        public abstract void RefreshConnectionStatus();
        public abstract void RefreshSpecs();
		public abstract SwitcherCompatibility GetPlatformCompatibility();
		public abstract void Dispose();

		public virtual void SetEventHandler(ISwitcherEventHandler? eventHandler)
		{
            // TODO: For safety, implement a non-null event handler target
            _eventHandler = eventHandler;
		}
	}
}
