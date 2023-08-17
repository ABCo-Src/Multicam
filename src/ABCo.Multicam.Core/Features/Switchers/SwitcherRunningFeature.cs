﻿using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public interface ISwitcherRunningFeature : ILiveFeature
    {
        SwitcherSpecs SwitcherSpecs { get; }
        int GetValue(int mixBlock, int bus);
        void PostValue(int mixBlock, int bus, int value);
        void Cut(int mixBlock);
    }

    public interface IBinderForSwitcherFeature : ILiveFeatureBinder, INeedsInitialization<ISwitcherRunningFeature>
    {
        void ModelChange_Specs();
        void ModelChange_BusValues();
    }

    public class SwitcherRunningFeature : ISwitcherRunningFeature
    {
        // The raw underlying switcher
        //ISwitcher _rawSwitcher;

        // TODO: Add slamming protection
        // TODO: Add error handling

        // The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
        // A new one is created anytime the specs change (which is why it's broken into its own service, it's an easy way to avoid async data tearing when switcher (specs) are changed).
        ISwitcherInteractionBuffer _buffer;
        readonly ISwitcherInteractionBufferFactory _bufferFactory;

        public bool IsConnected => _buffer.IsConnected;
        public SwitcherSpecs SwitcherSpecs => _buffer.Specs;

        readonly IBinderForSwitcherFeature _uiBinder;
        public ILiveFeatureBinder UIBinder => _uiBinder;

        public FeatureTypes FeatureType => FeatureTypes.Switcher;

        public SwitcherRunningFeature(IDummySwitcher dummySwitcher, ISwitcherInteractionBufferFactory bufferFactory, IBinderForSwitcherFeature uiBinder)
        {
            _bufferFactory = bufferFactory;
            _uiBinder = uiBinder;
            _buffer = bufferFactory.CreateSync(dummySwitcher);
            _buffer.SetOnBusChangeFinishCall(OnBusChange);

            _uiBinder.FinishConstruction(this);
        }

        public int GetValue(int mixBlock, int bus) => _buffer.GetValue(mixBlock, bus);
        public void PostValue(int mixBlock, int bus, int value) => _buffer.PostValue(mixBlock, bus, value);

        public async Task ChangeSwitcherAsync(ISwitcher switcher)
        {
            var oldBuffer = _buffer;

            _buffer = await _bufferFactory.CreateAsync(switcher);
            _buffer.SetOnBusChangeFinishCall(OnBusChange);

            oldBuffer.Dispose();
        }

        void OnBusChange(RetrospectiveFadeInfo? info) => _uiBinder.ModelChange_BusValues();

        public void Dispose() => _buffer.Dispose();
        public void Cut(int mixBlock) => _buffer.Cut(mixBlock);
    }
}
