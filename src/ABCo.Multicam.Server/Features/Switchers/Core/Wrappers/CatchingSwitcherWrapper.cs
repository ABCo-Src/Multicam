using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.Wrappers
{
    /// <summary>
    /// Catches all the given operations
    /// </summary>
    public class CatchingSwitcherWrapper : PassthroughSwitcherBase
    {
        public CatchingSwitcherWrapper(IRawSwitcher nextSwitcher) : base(nextSwitcher) { }

        public override void Connect()
        {
            try { _nextSwitcher.Connect(); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override void Disconnect()
        {
            try { _nextSwitcher.Disconnect(); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override void Cut(int mixBlock)
        {
            try { _nextSwitcher.Cut(mixBlock); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override SwitcherPlatformCompatibilityValue GetPlatformCompatibility()
        {
            try { return _nextSwitcher.GetPlatformCompatibility(); }
            catch (Exception ex) { HandleError(ex); return SwitcherPlatformCompatibilityValue.Supported; }
        }

        public override void RefreshConnectionStatus()
        {
            try { _nextSwitcher.RefreshConnectionStatus(); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override void RefreshPreview(int mixBlock)
        {
            try { _nextSwitcher.RefreshPreview(mixBlock); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override void RefreshProgram(int mixBlock)
        {
            try { _nextSwitcher.RefreshProgram(mixBlock); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override void RefreshSpecs()
        {
            try { _nextSwitcher.RefreshSpecs(); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override void SendPreviewValue(int mixBlock, int id)
        {
            try { _nextSwitcher.SendPreviewValue(mixBlock, id); }
            catch (Exception ex) { HandleError(ex); }
        }

        public override void SendProgramValue(int mixBlock, int id) 
        {
            try { _nextSwitcher.SendProgramValue(mixBlock, id); }
            catch (Exception ex) { HandleError(ex); }
        }

        // Dispose is excluded as those errors are actually a concern...

        void HandleError(Exception ex) => _parentSwitcher?.OnFailure(new(ex.Message));
    }
}
