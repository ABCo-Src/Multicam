using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.ViewModels.Features;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.Bindings.Features
{
    [TestClass]
    public class FeatureBinderTests : VMBinderBaseTest<FeatureVMBinder, IVMForFeatureBinder, IFeatureContainer>
    {
        Mock<IFeatureManager> _manager = new();

        public override VMTestProperty[] Props => new VMTestProperty[]
        {
            new(nameof(IVMForFeatureBinder.RawManager), null, null, vm => vm.RawManager = _manager.Object),
            new(nameof(IVMForFeatureBinder.RawFeature), null, null, vm => vm.RawFeature = _mocks.Model.Object)
        };

        public override void SetupModel(Mock<IFeatureContainer> model) { }
        public override FeatureVMBinder Create()
        {
            var vm = new FeatureVMBinder(Mock.Of<IServiceSource>());
            vm.FinishConstruction(_manager.Object, _mocks.Model.Object);
            return vm;
        }
    }
}