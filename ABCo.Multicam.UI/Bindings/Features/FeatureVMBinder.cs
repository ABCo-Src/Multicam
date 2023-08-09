using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings.Features
{
    public interface IBindableFeatureVM : IBindableVM
    {
        
    }

    public class FeatureVMBinder : VMBinder<IBindableFeatureVM>
    {
        public FeatureVMBinder(IServiceSource source) : base(source) { }

        public override void OnVMChange(IBindableFeatureVM vm, string? prop)
        {
            throw new NotImplementedException();
        }

        public override void RefreshVMToModel(IBindableFeatureVM vm)
        {
            throw new NotImplementedException();
        }
    }
}
