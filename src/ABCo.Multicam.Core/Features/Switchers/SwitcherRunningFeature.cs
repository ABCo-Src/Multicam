using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using System.Diagnostics.Contracts;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public interface ISwitcherRunningFeature : ILiveFeature
    {
        SwitcherConfig SwitcherConfig { get; }
        SwitcherSpecs SwitcherSpecs { get; }
        int GetProgram(int mixBlock);
        int GetPreview(int mixBlock);
        void SendProgram(int mixBlock, int value);
        void SendPreview(int mixBlock, int value);
        void Cut(int mixBlock);
        void ChangeSwitcher(SwitcherConfig config);
    }

    public interface ISwitcherEventHandler
    {
        void OnProgramValueChange(SwitcherProgramChangeInfo info);
        void UpdatePreview(SwitcherPreviewChangeInfo info);
        void OnSpecsChange(SwitcherSpecs newSpecs);
        void OnConnectionStateChange(bool isConnected);
    }

    public interface IBinderForSwitcherFeature : ILiveFeatureBinder, INeedsInitialization<ISwitcherRunningFeature>
    {
        void ModelChange_Specs();
        void ModelChange_Config();
        void ModelChange_BusValues();
        void ModelChange_ConnectionState();
    }

    public class SwitcherRunningFeature : ISwitcherRunningFeature, ISwitcherEventHandler
    {
        // TODO: Add slamming protection
        // TODO: Add error handling

        // The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
        // A new interaction buffer is created anytime the specs change, and the swapper facilitates for us.
        readonly IHotSwappableSwitcherInteractionBuffer _buffer;
        readonly IBinderForSwitcherFeature _uiBinder;

        public SwitcherRunningFeature(IServiceSource serviceSource)
        {
            _buffer = serviceSource.Get<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>(SwitcherConfig = new DummySwitcherConfig(4));
            _uiBinder = serviceSource.Get<IBinderForSwitcherFeature, ISwitcherRunningFeature>(this);
            _buffer.SetEventHandler(this);
        }

        // Properties:
        public FeatureTypes FeatureType => FeatureTypes.Switcher;
        public ILiveFeatureBinder UIBinder => _uiBinder;
        public bool IsConnected => _buffer.CurrentBuffer.IsConnected;
        public SwitcherSpecs SwitcherSpecs => _buffer.CurrentBuffer.Specs;
        public SwitcherConfig SwitcherConfig { get; private set; }

        // Methods:
        public int GetProgram(int mixBlock) => _buffer.CurrentBuffer.GetProgram(mixBlock);
        public int GetPreview(int mixBlock) => _buffer.CurrentBuffer.GetPreview(mixBlock);
        public void SendProgram(int mixBlock, int value) => _buffer.CurrentBuffer.SendProgram(mixBlock, value);
        public void SendPreview(int mixBlock, int value) => _buffer.CurrentBuffer.SendPreview(mixBlock, value);
        public void ChangeSwitcher(SwitcherConfig config)
        {
            _buffer.ChangeSwitcher(config);
            SwitcherConfig = config;
            _uiBinder.ModelChange_Config();
        }

        public void Cut(int mixBlock) => _buffer.CurrentBuffer.Cut(mixBlock);

        // Events:
        public void OnProgramValueChange(SwitcherProgramChangeInfo info) => _uiBinder.ModelChange_BusValues();
        public void UpdatePreview(SwitcherPreviewChangeInfo info) => _uiBinder.ModelChange_BusValues();
        public void OnSpecsChange(SwitcherSpecs newSpecs) => _uiBinder.ModelChange_Specs();
		public void OnConnectionStateChange(bool newState) => _uiBinder.ModelChange_ConnectionState();

		// Dispose:
		public void Dispose() => _buffer.Dispose();
    }
}
