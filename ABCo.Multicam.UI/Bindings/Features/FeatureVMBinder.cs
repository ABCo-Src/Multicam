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

    public interface IBinderForFeature : IVMBinder<IVMForFeatureBinder> 
    {
        void FinishConstruction(IFeatureManager manager, IFeatureContainer feature);
    }

    public class FeatureVMBinder : VMBinder<IVMForFeatureBinder>, IBinderForFeature
    {
        IFeatureManager _manager = null!;
        IFeatureContainer _feature = null!;

        public FeatureVMBinder(IServiceSource source) : base(source) { }
        public void FinishConstruction(IFeatureManager manager, IFeatureContainer feature)
        {
            _manager = manager;
            _feature = feature;
        }

        public override void OnVMChange(IVMForFeatureBinder vm, string? prop) { }

        public override void RefreshVMToModel(IVMForFeatureBinder vm)
        {
            vm.RawManager = _manager;
            vm.RawFeature = _feature;
        }
    }
}
