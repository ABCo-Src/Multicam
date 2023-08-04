using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Features;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features
{
    public class DummyFeatureViewModel : FeatureViewModel
    {
        public IServiceSource Source => _serviceSource;
        public override IRunningFeature BaseFeature => throw new NotImplementedException();
        public override FeatureViewType ContentView => FeatureViewType.Unsupported;
        public DummyFeatureViewModel(IServiceSource serviceSource, IProjectFeaturesViewModel parent) : base(serviceSource, parent) { }
    }

    [TestClass]
    public class FeatureViewModelTests
    {
        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new DummyFeatureViewModel(null!, Mock.Of<IProjectFeaturesViewModel>()));

        [TestMethod]
        public void Ctor()
        {
            var parent = Mock.Of<IProjectFeaturesViewModel>();
            var serviceSource = Mock.Of<IServiceSource>();
            var vm = new DummyFeatureViewModel(serviceSource, parent);

            Assert.AreEqual("New Feature", vm.FeatureTitle);
            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(serviceSource, vm.Source);
            Assert.AreEqual(false, vm.IsEditing);
        }

        [TestMethod]
        public void ToggleEdit_NotEditing()
        {
            var parent = new Mock<IProjectFeaturesViewModel>();
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), parent.Object);

            vm.ToggleEdit();

            parent.VerifySet(i => i.CurrentlyEditing = vm);
        }

        [TestMethod]
        public void ToggleEdit_Editing()
        {
            var parent = new Mock<IProjectFeaturesViewModel>();
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), parent.Object);

            vm.IsEditing = true;
            vm.ToggleEdit();

            parent.VerifySet(i => i.CurrentlyEditing = null);
        }

        [TestMethod]
        public void EditBtnText_NotEditing()
        {
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), Mock.Of<IProjectFeaturesViewModel>());
            vm.IsEditing = false;
            Assert.AreEqual("Edit", vm.EditBtnText);
        }

        [TestMethod]
        public void EditBtnText_Editing()
        {
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), Mock.Of<IProjectFeaturesViewModel>());
            vm.IsEditing = true;
            Assert.AreEqual("Finish", vm.EditBtnText);
        }


        [TestMethod]
        public void EditPanelTitle()
        {
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), Mock.Of<IProjectFeaturesViewModel>());
            vm.FeatureTitle = "abc";
            Assert.AreEqual("Editing 'abc'", vm.EditPanelTitle);
        }

        [TestMethod]
        public void MoveDown()
        {
            var parent = new Mock<IProjectFeaturesViewModel>();
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), parent.Object);
            vm.MoveDown();
            parent.Verify(i => i.MoveDown(vm));
        }

        [TestMethod]
        public void MoveUp()
        {
            var parent = new Mock<IProjectFeaturesViewModel>();
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), parent.Object);
            vm.MoveUp();
            parent.Verify(i => i.MoveUp(vm));
        }

        [TestMethod]
        public void Delete()
        {
            var parent = new Mock<IProjectFeaturesViewModel>();
            var vm = new DummyFeatureViewModel(Mock.Of<IServiceSource>(), parent.Object);
            vm.Delete();
            parent.Verify(i => i.Delete(vm));
        }

        [TestMethod]
        public void Unknown_Ctor()
        {
            var parent = Mock.Of<IProjectFeaturesViewModel>();
            var serviceSource = Mock.Of<IServiceSource>();
            var model = Mock.Of<IRunningFeature>();
            var vm = new UnsupportedFeatureViewModel(new NewViewModelInfo(model, parent), serviceSource);

            Assert.AreEqual(FeatureViewType.Unsupported, vm.ContentView);
            Assert.AreEqual(model, vm.BaseFeature);
        }

        [TestMethod]
        public void IsEditing_UpdatesEditBtnText()
        {
            // TODO: Create viewmodel consistency tests
        }
    }
}
