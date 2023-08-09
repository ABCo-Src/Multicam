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
    public class ProjectFeaturesBinderTest : VMBinderBaseTest<ProjectFeaturesVMBinder, IVMForProjectFeaturesBinder, IFeatureManager>
    {
        public override VMTestProperty[] Props => new VMTestProperty[]
        {
            new(nameof(IVMForProjectFeaturesBinder.RawFeatures), model => model.ModelChange_FeaturesChange(), null, vm => vm.RawFeatures = It.IsAny<IFeatureVMBinder[]>())
        };

        public override void SetupModel(Mock<IFeatureManager> model) => model.Setup(m => m.Features).Returns(new IRunningFeature[2]);
        public override ProjectFeaturesVMBinder Create() => new ProjectFeaturesVMBinder(_mocks.Model.Object, Mock.Of<IServiceSource>());
    }
}