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
    public class ProjectFeaturesBinderTests : VMBinderBaseTest<ProjectFeaturesVMBinder, IVMForProjectFeaturesBinder, IFeatureManager>
    {
        public override VMTestProperty[] Props => new VMTestProperty[]
        {
            new(nameof(IVMForProjectFeaturesBinder.RawFeatures), model => model.ModelChange_FeaturesChange(), null, vm => vm.RawFeatures = It.IsAny<IBinderForFeature[]>()),
            new(nameof(IVMForProjectFeaturesBinder.RawManager), null, null, vm => vm.RawManager = It.IsAny<IFeatureManager>())
        };

        public override void SetupModel(Mock<IFeatureManager> model) => model.Setup(m => m.Features).Returns(new IFeatureContainer[2]);
        public override ProjectFeaturesVMBinder Create()
        {
            var vm = new ProjectFeaturesVMBinder(Mock.Of<IServiceSource>());
            vm.FinishConstruction(_mocks.Model.Object);
            return vm;
        }
    }
}