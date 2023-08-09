using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings.Features
{
    public interface IVMForProjectFeaturesBinder : IBindableVM<IVMForProjectFeaturesBinder>
    {
        IFeatureVMBinder[] RawFeatures { get; set; }
        IFeatureManager RawManager { get; set; }
    }

    public interface IFeatureVMBinder { }

    public class ProjectFeaturesVMBinder : VMBinder<IVMForProjectFeaturesBinder>, IBinderForProjectFeatures
    {
        IFeatureManager _model;
        IServiceSource _servSource;
        IFeatureVMBinder[] _rawFeatures = Array.Empty<IFeatureVMBinder>();

        public ProjectFeaturesVMBinder(IFeatureManager model, IServiceSource source) : base(source)
        {
            _model = model;
            _servSource = source;
        }

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
