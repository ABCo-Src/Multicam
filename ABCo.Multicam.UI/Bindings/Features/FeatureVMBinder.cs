using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings.Features
{
    public interface IVMForFeatureBinder : IVMForBinder<IVMForFeatureBinder> 
    { 
        IFeatureManager RawManager { get; set; }
        IFeatureContainer RawFeature { get; set; }
    }

    public class FeatureVMBinder : VMBinder<IVMForFeatureBinder>, IBinderForFeatureContainer
    {
        IFeatureManager _manager = null!;
        IFeatureContainer _feature = null!;

        public override PropertyBinding[] CreateProperties() => new PropertyBinding[]
        {
            // RawManager
            new PropertyBinding<IFeatureManager>()
            {
                ModelChange = new(() => _manager, v => v.VM.RawManager = v.NewVal)
            },

            // RawFeature
            new PropertyBinding<IFeatureContainer>()
            {
                ModelChange = new(() => _feature, v => v.VM.RawFeature = v.NewVal)
            },
        };

        public FeatureVMBinder(IServiceSource source) : base(source) { }
        public void FinishConstruction(IFeatureManager manager, IFeatureContainer feature)
        {
            _manager = manager;
            _feature = feature;
            Init();
        }
    }
}
