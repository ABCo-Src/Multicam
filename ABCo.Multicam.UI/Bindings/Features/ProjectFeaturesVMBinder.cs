using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings.Features
{
    public interface IVMForProjectFeaturesBinder : IVMForBinder<IVMForProjectFeaturesBinder>
    {
        IFeatureVMBinder[] RawFeatures { get; set; }
        IFeatureManager RawManager { get; set; }
    }

    public interface IVMForFeatureBinder : IVMForBinder<IVMForFeatureBinder> { }
    public interface IFeatureVMBinder : IVMBinder<IVMForFeatureBinder> { }

    public class ProjectFeaturesVMBinder : VMBinder<IVMForProjectFeaturesBinder>, IBinderForProjectFeatures
    {
        IFeatureManager _model = null!;
        IFeatureVMBinder[] _rawFeatures = Array.Empty<IFeatureVMBinder>();

        public ProjectFeaturesVMBinder(IServiceSource source) : base(source) { }
        public void FinishConstruction(IFeatureManager model) => _model = model;

        public void ModelChange_FeaturesChange()
        {
            // TODO: I think we need a unit testing framework for this...
            _rawFeatures = new IFeatureVMBinder[_model.Features.Count];
            for (int i = 0; i < _model.Features.Count; i++)
                _rawFeatures[i] = _servSource.GetWithParameter<IFeatureVMBinder, IRunningFeature>(_model.Features[i]);

            SetVMProp(vm => vm.RawFeatures = _rawFeatures, nameof(IVMForProjectFeaturesBinder.RawFeatures));
        }

        public override void OnVMChange(IVMForProjectFeaturesBinder vm, string? prop) { }

        public override void RefreshVMToModel(IVMForProjectFeaturesBinder vm)
        {
            vm.RawManager = _model;
            ModelChange_FeaturesChange();
        }
    }
}
