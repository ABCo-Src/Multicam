using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using Moq;

namespace ABCo.Multicam.Tests.UI.Bindings.Features
{
    [TestClass]
    public class ProjectFeaturesBinderTests
    {
        public interface SubBinder : IBinderForFeatureContainer, IVMBinder<IVMForFeatureBinder> { }

        record struct Mocks(Mock<IFeatureManager> FeatureManager, Mock<SubBinder>[] Binders);
        Mocks _mocks = new();

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.Binders = new Mock<SubBinder>[] { new(), new() };

            _mocks.FeatureManager = new();
            _mocks.FeatureManager.Setup(m => m.Features).Returns(new IFeatureContainer[] 
            { 
                Mock.Of<IFeatureContainer>(m => m.UIBinder == _mocks.Binders[0].Object), 
                Mock.Of<IFeatureContainer>(m => m.UIBinder == _mocks.Binders[1].Object) 
            });
        }

        public ProjectFeaturesVMBinder Create()
        {
            var vm = new ProjectFeaturesVMBinder(Mock.Of<IServiceSource>());
            vm.FinishConstruction(_mocks.FeatureManager.Object);
            return vm;
        }

        [TestMethod]
        public void GetFeatureBinders()
        {
            var arr = Create().GetFeatureBinders();
            Assert.AreEqual(2, arr.Length);
            Assert.AreEqual(_mocks.Binders[0].Object, arr[0]);
            Assert.AreEqual(_mocks.Binders[1].Object, arr[1]);
        }

        // TODO: Test ModelChanges somehow?
    }
}