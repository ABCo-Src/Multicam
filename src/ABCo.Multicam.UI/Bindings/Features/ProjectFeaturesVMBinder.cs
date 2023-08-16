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
        IVMBinder<IVMForFeatureBinder>[] RawFeatures { get; set; }
        IFeatureManager RawManager { get; set; }
    }

    public class ProjectFeaturesVMBinder : VMBinder<IVMForProjectFeaturesBinder>, IBinderForProjectFeatures
    {
        IFeatureManager _model = null!;

        public override PropertyBinding[] CreateProperties() => new PropertyBinding[]
        {
            // RawManager
            new PropertyBinding<IFeatureManager>()
            {
                ModelChange = new(() => _model, v => v.VM.RawManager = v.NewVal)
            },

            // RawFeatures
            new PropertyBinding<IVMBinder<IVMForFeatureBinder>[]>()
            {
                ModelChange = new(GetFeatureBinders, v => 
                    v.VM.RawFeatures = v.NewVal)
            }
        };

        public IVMBinder<IVMForFeatureBinder>[] GetFeatureBinders()
        {
            var arr = new IVMBinder<IVMForFeatureBinder>[_model.Features.Count];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = (IVMBinder<IVMForFeatureBinder>)_model.Features[i].UIBinder;

            return arr;
        }

        public void ModelChange_FeaturesChange() => ReportModelChange(Properties[1]);

        public ProjectFeaturesVMBinder(IServiceSource source) : base(source) { }
        public void FinishConstruction(IFeatureManager model)
        {
            _model = model;
            Init();
        }
    }
}
