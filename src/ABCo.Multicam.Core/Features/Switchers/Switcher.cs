using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public class UnsupportedSwitcherBehaviourException : Exception
    {
        public UnsupportedSwitcherBehaviourException() : base("Switcher was asked to perform something it can't. Either switcher is reporting incorrect specs or something higher-up went wrong.") { }
    }

    /// <summary>
    /// Provides a base class that fails everything on the ISwitcher API unless overriden.
    /// </summary>
    public abstract class Switcher : ISwitcher
    {
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
        public virtual void SetEventHandler(ISwitcherEventHandler? eventHandler) => throw new UnsupportedSwitcherBehaviourException();

        public abstract void RefreshConnectionStatus();
        public abstract void RefreshSpecs();
        public abstract void Dispose();
    }
}
