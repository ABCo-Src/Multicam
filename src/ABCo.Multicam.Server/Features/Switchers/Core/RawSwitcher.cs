using ABCo.Multicam.Server.Features.Switchers.Fading;

namespace ABCo.Multicam.Server.Features.Switchers.Core
{
    public class UnsupportedSwitcherBehaviourException : Exception
    {
        public UnsupportedSwitcherBehaviourException() : base("Switcher was asked to perform something it can't. Either switcher is reporting incorrect specs or something higher-up went wrong.") { }
    }

    /// <summary>
    /// The raw surface to talk to a switcher. 
    /// 
    /// None of these operations are likely to be cached (there's a buffer sitting atop to do that caching), and all interactions implemented here should only be
    /// implemented if the switcher can natively/intuitively perform them. 
    /// 
    /// For example, the preview API should throw if the specs say preview isn't supported. The layers above the switcher will emulate it.
    /// </summary>
    public interface IRawSwitcher : IDisposable
    {
        // General:
        void Connect();
        void Disconnect();
        void RefreshConnectionStatus();
        void RefreshSpecs();
        SwitcherPlatformCompatibilityValue GetPlatformCompatibility();

        // Program/Preview:
        void RefreshProgram(int mixBlock);
        void RefreshPreview(int mixBlock);
        void SendProgramValue(int mixBlock, int id);
        void SendPreviewValue(int mixBlock, int id);
        void Cut(int mixBlock);

        // Event Handling:
        void SetEventHandler(ISwitcherEventHandler? eventHandler);
    }

    public enum CutBusMode
    {
        Cut,
        Auto
    }

    public record struct SwitcherProgramChangeInfo(int MixBlock, int NewValue, RetrospectiveFadeInfo? FadeInfo);
    public record struct SwitcherPreviewChangeInfo(int MixBlock, int NewValue, RetrospectiveFadeInfo? FadeInfo);

    /// <summary>
    /// Provides a base class to make implementing switchers easier that:
    /// 1. Fails everything on the ISwitcher API unless overriden.
    /// 2. Stores the assigned event handler in a protected field
    /// </summary>
    public abstract class RawSwitcher : IRawSwitcher
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
        public abstract SwitcherPlatformCompatibilityValue GetPlatformCompatibility();
        public abstract void Dispose();

        public virtual void SetEventHandler(ISwitcherEventHandler? eventHandler)
        {
            // TODO: For safety, implement a non-null event handler target
            _eventHandler = eventHandler;
        }
    }
}
