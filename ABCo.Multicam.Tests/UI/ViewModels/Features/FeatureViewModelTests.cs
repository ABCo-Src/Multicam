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
    [TestClass]
    public class FeatureViewModelTests
    {
        public record struct Mocks(Mock<IServiceSource> ServSource, Mock<IFeatureManager> FeatureManager, Mock<IFeatureContainer> RawFeature, Mock<IProjectFeaturesViewModel> Parent);

        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.ServSource = new();
            _mocks.FeatureManager = new();
            _mocks.RawFeature = new();
            _mocks.Parent = new();
        }

        public FeatureViewModel Create() => new(_mocks.ServSource.Object)
        {
            Parent = _mocks.Parent.Object,
            RawManager = _mocks.FeatureManager.Object,
            RawFeature = _mocks.RawFeature.Object
        };

        [TestMethod]
        public void Ctor()
        {
            var vm = Create();
            Assert.AreEqual("New Feature", vm.FeatureTitle);
            Assert.AreEqual(false, vm.IsEditing);
        }

        [TestMethod]
        public void ToggleEdit_NotEditing()
        {
            var vm = Create();
            vm.ToggleEdit();
            _mocks.Parent.VerifySet(i => i.CurrentlyEditing = vm);
        }

        [TestMethod]
        public void ToggleEdit_Editing()
        {
            var vm = Create();
            vm.IsEditing = true;
            vm.ToggleEdit();

            _mocks.Parent.VerifySet(i => i.CurrentlyEditing = null);
        }

        [TestMethod]
        public void EditBtnText_NotEditing()
        {
            var vm = Create();
            vm.IsEditing = false;
            Assert.AreEqual("Edit", vm.EditBtnText);
        }

        [TestMethod]
        public void EditBtnText_Editing()
        {
            var vm = Create();
            vm.IsEditing = true;
            Assert.AreEqual("Finish", vm.EditBtnText);
        }


        [TestMethod]
        public void EditPanelTitle()
        {
            var vm = Create();
            vm.FeatureTitle = "abc";
            Assert.AreEqual("Editing 'abc'", vm.EditPanelTitle);
        }

        [TestMethod]
        public void MoveDown()
        {
            Create().MoveDown();
            _mocks.FeatureManager.Verify(i => i.MoveDown(_mocks.RawFeature.Object));
        }

        [TestMethod]
        public void MoveUp()
        {
            Create().MoveUp();
            _mocks.FeatureManager.Verify(i => i.MoveUp(_mocks.RawFeature.Object));
        }

        [TestMethod]
        public void Delete()
        {
            Create().Delete();
            _mocks.FeatureManager.Verify(i => i.Delete(_mocks.RawFeature.Object));
        }

        //[TestMethod]
        //public void Unknown_Ctor()
        //{
        //    var parent = Mock.Of<IProjectFeaturesViewModel>();
        //    var serviceSource = Mock.Of<IServiceSource>();
        //    var model = Mock.Of<IRunningFeature>();
        //    var vm = new UnsupportedFeatureViewModel(new NewViewModelInfo(model, parent), serviceSource);

        //    Assert.AreEqual(FeatureViewType.Unsupported, vm.ContentView);
        //    Assert.AreEqual(model, vm.BaseFeature);
        //}

        [TestMethod]
        public void IsEditing_UpdatesEditBtnText()
        {
            // TODO: Create viewmodel consistency tests
        }
    }
}
