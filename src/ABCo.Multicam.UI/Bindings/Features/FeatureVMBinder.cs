using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;

namespace ABCo.Multicam.UI.Bindings.Features
{
    public interface IVMForFeatureBinder : IVMForBinder<IVMForFeatureBinder> 
    { 
        IFeatureManager RawManager { get; set; }
        IFeatureContainer RawContainer { get; set; }
        ILiveFeature RawInnerFeature { get; set; }
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
                ModelChange = new(() => _feature, v => v.VM.RawContainer = v.NewVal)
            },

            // InnerFeature
            new PropertyBinding<ILiveFeature>()
            {
                ModelChange = new(() => _feature.CurrentFeature, v => v.VM.RawInnerFeature = v.NewVal)
            }
        };

        public FeatureVMBinder(IServiceSource source) : base(source) { }
        public void FinishConstruction(IFeatureManager manager, IFeatureContainer feature)
        {
            _manager = manager;
            _feature = feature;
            Init();
        }
    }

    public class UnsupportedFeatureVMBinder : IBinderForUnsupportedFeature { }
}
