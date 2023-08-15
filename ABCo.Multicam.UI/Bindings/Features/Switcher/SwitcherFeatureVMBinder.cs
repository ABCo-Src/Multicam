using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
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

    public interface IVMForSwitcherMixBlock : IVMForBinder<IVMForSwitcherMixBlock>
    {

    }

    public interface IBinderForSwitcherMixBlock
    {
        void FinishConstruction(ISwitcherRunningFeature feature, SwitcherMixBlock block, int index);
    }

    public class SwitcherFeatureVMBinder : VMBinder<IVMForSwitcherFeature>, IBinderForSwitcherFeature
    {
        ISwitcherRunningFeature _feature = null!;

        public override PropertyBinding[] CreateProperties() => new PropertyBinding[]
        {
            // RawMixBlocks
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

        public IVMBinder<IVMForSwitcherMixBlock>[] GetMixBlocks()
        {
            var arr = new IVMBinder<IVMForSwitcherMixBlock>[_feature.SwitcherSpecs.MixBlocks.Count];

            for (int i = 0; i < arr.Length; i++)
            {
                var newVal = _servSource.Get<IBinderForSwitcherMixBlock>();
                newVal.FinishConstruction(_feature, _feature.SwitcherSpecs.MixBlocks[i], i);
                arr[i] = (IVMBinder<IVMForSwitcherMixBlock>)newVal;
            }

            return arr;
        }

        public void ModelChange_Specs() => ReportModelChange(Properties[0]);
    }
}
