using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings.Features.Switcher
{
    public interface IVMForSwitcherFeature : IVMForBinder<IVMForSwitcherFeature>
    {

    }

    public interface IVMForSwitcherMixBlock : IVMForBinder<IVMForSwitcherMixBlock>
    {

    }

    public class SwitcherFeatureVMBinder : VMBinder<IVMForSwitcherFeature>
    {
        public SwitcherFeatureVMBinder(IServiceSource servSource) : base(servSource) { }
    }
}
