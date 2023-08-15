using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features
{
    [TestClass]
    public class FeatureViewModelTests
    {
        public interface ISwitcherBinder : IVMBinder<IVMForSwitcherFeature>, ILiveFeatureBinder { }

        public record struct Mocks(
            Mock<IFeatureManager> FeatureManager, 
            Mock<IFeatureContainer> RawFeature, 
            Mock<IProjectFeaturesViewModel> Parent,
            Mock<ILiveFeature> InnerFeature,
            Mock<ISwitcherBinder> InnerFeatureBinder,
            Mock<ISwitcherFeatureVM> SwitcherVM,
            Mock<ISwitcherFeatureVM> UnsupportedVM);

        FeatureTypes _type;
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.FeatureManager = new();
            _mocks.SwitcherVM = new();
            _mocks.UnsupportedVM = new();

            _mocks.InnerFeatureBinder = new();
            _mocks.InnerFeatureBinder.Setup(m => m.GetVM<ISwitcherFeatureVM>(It.IsAny<object>())).Returns(_mocks.SwitcherVM.Object);

            _mocks.InnerFeature = new();
            _mocks.InnerFeature.SetupGet(m => m.UIBinder).Returns(_mocks.InnerFeatureBinder.Object);
            _mocks.InnerFeature.SetupGet(m => m.FeatureType).Returns(() => _type);

            _mocks.RawFeature = new();
            _mocks.Parent = new();
        }

        public FeatureViewModel Create() => new()
        {
            Parent = _mocks.Parent.Object,
            RawManager = _mocks.FeatureManager.Object,
            RawContainer = _mocks.RawFeature.Object,
            RawInnerFeature = _mocks.InnerFeature.Object
        };

        [TestMethod]
        public void Ctor()
        {
            var vm = Create();
            Assert.AreEqual("New Feature", vm.FeatureTitle);
            Assert.AreEqual(false, vm.IsEditing);
        }

        [TestMethod]
        public void Content_Switcher()
        {
            _type = FeatureTypes.Switcher;
            var vm = Create();
            Assert.AreEqual(_mocks.SwitcherVM.Object, vm.InnerVM);
            _mocks.InnerFeatureBinder.Verify(m => m.GetVM<ISwitcherFeatureVM>(vm), Times.Once);
        }

        [TestMethod]
        public void Content_Unsupported()
        {
            _type = FeatureTypes.Unsupported;
            var vm = Create();
            Assert.IsInstanceOfType(vm.InnerVM, typeof(UnsupportedFeatureViewModel));
        }

        [TestMethod]
        [DataRow(FeatureTypes.Switcher)]
        [DataRow(FeatureTypes.Unsupported)]
        public void ContentType(FeatureTypes type)
        {
            _type = type;
            Assert.AreEqual(type, Create().InnerType);
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
