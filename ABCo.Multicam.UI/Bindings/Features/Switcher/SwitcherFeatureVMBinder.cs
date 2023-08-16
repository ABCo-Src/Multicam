using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings.Features.Switcher
{
    public interface IVMForSwitcherFeature : IVMForBinder<IVMForSwitcherFeature>
    {
        IVMBinder<IVMForSwitcherMixBlock>[] RawMixBlocks { get; set; }
    }

    public class SwitcherFeatureVMBinder : VMBinder<IVMForSwitcherFeature>, IBinderForSwitcherFeature
    {
        ISwitcherRunningFeature _feature = null!;

        public override PropertyBinding[] CreateProperties() => new PropertyBinding[]
        {
            // RawMixBlock
            new PropertyBinding<IVMBinder<IVMForSwitcherMixBlock>[]>()
            {
                ModelChange = new(GetMixBlocks, v => v.VM.RawMixBlocks = v.NewVal)
            }
        };

        public SwitcherFeatureVMBinder(IServiceSource servSource) : base(servSource) { }
        public void FinishConstruction(ISwitcherRunningFeature feature) 
        {
            _feature = feature;
            Init();
        }

        IVMBinder<IVMForSwitcherMixBlock>[] _currentMixBlocks;
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

        public void ModelChange_Specs() => ReportModelChange(Properties[0]);
        public void ModelChange_BusValues()
        {
            for (int i = 0; i < _currentMixBlocks.Length; i++)
                ((IBinderForSwitcherMixBlock)_currentMixBlocks[i]).ModelChange_BusValues();
        }
    }
}
