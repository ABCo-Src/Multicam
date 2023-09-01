﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;

namespace ABCo.Multicam.UI.Bindings.Features.Switcher
{
    public interface IVMForSwitcherFeature : IVMForBinder<IVMForSwitcherFeature>
    {
        ISwitcherRunningFeature RawFeature { get; set; }
        IVMBinder<IVMForSwitcherMixBlock>[] RawMixBlocks { get; set; }
        SwitcherConfig RawConfig { get; set; }
        void UpdateConfig(SwitcherConfig config);
    }

    public class SwitcherFeatureVMBinder : VMBinder<IVMForSwitcherFeature>, IBinderForSwitcherFeature
    {
        ISwitcherRunningFeature _feature = null!;

        public override PropertyBinding[] CreateProperties() => new PropertyBinding[]
        {
            // RawFeature
            new PropertyBinding<ISwitcherRunningFeature>()
            {
                ModelChange = new(() => _feature, v => v.VM.RawFeature = v.NewVal)
            },

            // RawMixBlock
            new PropertyBinding<IVMBinder<IVMForSwitcherMixBlock>[]>()
            {
                ModelChange = new(GetMixBlocks, v => v.VM.RawMixBlocks = v.NewVal)
            },

            // RawConfig
            new PropertyBinding<SwitcherConfig>()
            {
                ModelChange = new(() => _feature.SwitcherConfig, v => v.VM.RawConfig = v.NewVal)
            }
        };

        public SwitcherFeatureVMBinder(IServiceSource servSource) : base(servSource) { }
        public void FinishConstruction(ISwitcherRunningFeature feature) 
        {
            _feature = feature;
            Init();
        }

        IVMBinder<IVMForSwitcherMixBlock>[] _currentMixBlocks = null!;
        public IVMBinder<IVMForSwitcherMixBlock>[] GetMixBlocks()
        {
            var arr = new IVMBinder<IVMForSwitcherMixBlock>[_feature.SwitcherSpecs.MixBlocks.Count];

            for (int i = 0; i < arr.Length; i++)
            {
                var newVal = _servSource.Get<IBinderForSwitcherMixBlock>();
                newVal.FinishConstruction(_feature, _feature.SwitcherSpecs.MixBlocks[i], i);
                arr[i] = (IVMBinder<IVMForSwitcherMixBlock>)newVal;
            }

            _currentMixBlocks = arr;
            return arr;
        }

        public void ModelChange_Specs() => ReportModelChange(Properties[1]);
        public void ModelChange_Config() => ReportModelChange(Properties[2]);
        public void ModelChange_ConnectionState() { }
        public void ModelChange_BusValues()
        {
            for (int i = 0; i < _currentMixBlocks.Length; i++)
                ((IBinderForSwitcherMixBlock)_currentMixBlocks[i]).ModelChange_BusValues();
        }
    }
}
