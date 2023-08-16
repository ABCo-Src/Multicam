using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings.Features.Switcher
{
    public interface IBinderForSwitcherMixBlock
    {
        void ModelChange_Bus();
        void FinishConstruction(ISwitcherRunningFeature feature, SwitcherMixBlock block, int index);
    }

    public interface IVMForSwitcherMixBlock : IVMForBinder<IVMForSwitcherMixBlock>
    {
        SwitcherMixBlock RawMixBlock { get; set; }
        int RawMixBlockIndex { get; set; }
        ISwitcherRunningFeature RawFeature { get; set; }
        int RawProgram { get; set; }
        int RawPreview { get; set; }
    }

    public class MixBlockVMBinder : VMBinder<IVMForSwitcherMixBlock>, IBinderForSwitcherMixBlock
    {
        ISwitcherRunningFeature _feature = null!;
        SwitcherMixBlock _mixBlock = null!;
        int _index;

        public override PropertyBinding[] CreateProperties() => new PropertyBinding[]
        {
            // RawFeature
            new PropertyBinding<ISwitcherRunningFeature>()
            {
                ModelChange = new(() => _feature, v => v.VM.RawFeature = v.NewVal)
            },

            // RawMixBlock
            new PropertyBinding<SwitcherMixBlock>()
            {
                ModelChange = new(() => _mixBlock, v => v.VM.RawMixBlock = v.NewVal)
            },

            // RawMixBlockIndex
            new PropertyBinding<int>()
            {
                ModelChange = new(() => _index, v => v.VM.RawMixBlockIndex = v.NewVal)
            },

            // RawProgram
            new PropertyBinding<int>()
            {
                ModelChange = new(() => _feature.GetValue(_index, 0), v => v.VM.RawProgram = v.NewVal)
            },

            // RawPreview
            new PropertyBinding<int>()
            {
                ModelChange = new(() => _feature.GetValue(_index, 1), v => v.VM.RawPreview = v.NewVal)
            }
        };

        public MixBlockVMBinder(IServiceSource source) : base(source) { }
        public void FinishConstruction(ISwitcherRunningFeature feature, SwitcherMixBlock block, int index)
        {
            _feature = feature;
            _mixBlock = block;
            _index = index;
            Init();
        }

        public void ModelChange_Bus()
        {
            
        }
    }
}
