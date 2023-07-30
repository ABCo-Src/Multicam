using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.ViewModels.Features
{
    [TestClass]
    public class ProjectFeaturesViewModelTests
    {
        public record struct Mocks(
            Mock<IFeatureManager> Model,
            Mock<IServiceSource> ServiceSource,
            Mock<IUIDialogHandler> DialogHandler,
            Mock<IRunningFeature>[] RunningFeatures,
            Mock<ISwitcherFeatureVM> SwitcherVM,
            Mock<ISwitcherRunningFeature> SwitcherRunning,
            Mock<IUnsupportedFeatureViewModel> UnsupportedVM,
            Mock<IUnsupportedRunningFeature> UnsupportedRunning
        );

        Action<FeatureTypes> _dialogHandlerCallback = d => { };
        Action _stripsChangeCallback = () => { };
        List<IRunningFeature> _modelFeatures = new();
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _modelFeatures = new();

            _mocks.Model = new();
            _mocks.Model.SetupGet(m => m.Features).Returns(() => _modelFeatures);
            _mocks.Model.Setup(m => m.SetOnFeaturesChangeForVM(It.IsAny<Action>())).Callback<Action>(v => _stripsChangeCallback = v);

            _mocks.DialogHandler = new();
            _mocks.DialogHandler
                .Setup(a => a.OpenContextMenu(It.IsAny<ContextMenuDetails<FeatureTypes>>()))
                .Callback<ContextMenuDetails<FeatureTypes>>((details) => _dialogHandlerCallback = details.OnSelect);

            _mocks.RunningFeatures = new Mock<IRunningFeature>[] { new(), new() };
            _mocks.SwitcherVM = new();
            _mocks.UnsupportedVM = new();
            _mocks.SwitcherRunning = new();
            _mocks.UnsupportedRunning = new();

            _mocks.ServiceSource = new();
            _mocks.ServiceSource.Setup(m => m.Get<IUIDialogHandler>()).Returns(() => _mocks.DialogHandler.Object);
            _mocks.ServiceSource.Setup(m => m.GetVM<ISwitcherFeatureVM>(It.IsAny<NewViewModelInfo>())).Returns(() => _mocks.SwitcherVM.Object);
            _mocks.ServiceSource.Setup(m => m.GetVM<IUnsupportedFeatureViewModel>(It.IsAny<NewViewModelInfo>())).Returns(() => _mocks.UnsupportedVM.Object);
        }

        ProjectFeaturesViewModel Create() => new(_mocks.Model.Object, _mocks.ServiceSource.Object);

        [TestMethod]
        public void Ctor_InitializesLocal()
        {
            var vm = Create();
            Assert.IsNotNull(vm.Items);
            Assert.IsNull(vm.CurrentlyEditing);
            Assert.AreEqual(0, vm.Items.Count);
        }

        [TestMethod]
        public void Ctor_InitializesEventHandler()
        {
            Create();
            _mocks.Model.Verify(m => m.SetOnFeaturesChangeForVM(It.IsAny<Action>()), Times.Once);
        }

        [TestMethod]
        public void Ctor_UpdatesItems()
        {
            _modelFeatures = new() { Mock.Of<IRunningFeature>(), Mock.Of<IRunningFeature>() };

            var vm = Create();
            Assert.AreEqual(2, vm.Items.Count);
            Assert.AreEqual(_modelFeatures[0], vm.Items[0].BaseFeature);
            Assert.AreEqual(_modelFeatures[1], vm.Items[1].BaseFeature);
        }

        [TestMethod]
        public void CreateFeature_OpensDialog()
        {
            Create().CreateFeature();

            _mocks.DialogHandler.Verify(a => a.OpenContextMenu(It.Is<ContextMenuDetails<FeatureTypes>>(d =>
                d.Title == "Choose Type" &&
                d.OnSelect != null &&
                d.OnCancel == null &&
                d.Items.SequenceEqual(new ContextMenuItem<FeatureTypes>[]
                    {
                        new("Switcher", FeatureTypes.Switcher),
                        new("Tally", FeatureTypes.Tally)
                    })
                ))
            );
        }

        [TestMethod]
        [DataRow(FeatureTypes.Switcher)]
        [DataRow(FeatureTypes.Tally)]
        public void CreateFeature_OnChoose(FeatureTypes type)
        {
            Create().CreateFeature();
            _dialogHandlerCallback(type);
            _mocks.Model.Verify(m => m.CreateFeature(type), Times.Once);
        }

        [TestMethod]
        public void FeatureVMCreation_Switcher()
        {
            _modelFeatures.Add(_mocks.SwitcherRunning.Object);
            var vm = Create();
            Assert.AreEqual(_mocks.SwitcherVM.Object, vm.Items[0]);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherFeatureVM>(new(_mocks.SwitcherRunning.Object, vm)));
        }

        [TestMethod]
        public void FeatureVMCreation_Unsupported()
        {
            _modelFeatures.Add(_mocks.UnsupportedRunning.Object);
            var vm = Create();
            Assert.IsInstanceOfType(vm.Items[0], typeof(UnsupportedFeatureViewModel));
        }

        [TestMethod]
        public void FeaturesChange_AddToEnd()
        {
            var vm = Create();
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            _stripsChangeCallback();

            Assert.AreEqual(1, vm.Items.Count);
            Assert.AreEqual(_mocks.RunningFeatures[0].Object, vm.Items[0].BaseFeature);
        }

        [TestMethod]
        public void FeaturesChange_AddToStart()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);

            var vm = Create();
            _modelFeatures.Insert(0, _mocks.RunningFeatures[1].Object);
            _stripsChangeCallback();

            Assert.AreEqual(2, vm.Items.Count);
            Assert.AreEqual(_mocks.RunningFeatures[1].Object, vm.Items[0].BaseFeature);
            Assert.AreEqual(_mocks.RunningFeatures[0].Object, vm.Items[1].BaseFeature);
        }

        [TestMethod]
        public void FeaturesChange_RemoveFromStart()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            _modelFeatures.Add(_mocks.RunningFeatures[1].Object);

            var vm = Create();
            _modelFeatures.Remove(_mocks.RunningFeatures[0].Object);
            _stripsChangeCallback();

            Assert.AreEqual(1, vm.Items.Count);
            Assert.AreEqual(_mocks.RunningFeatures[1].Object, vm.Items[0].BaseFeature);
        }

        [TestMethod]
        public void FeaturesChange_Remove_Editing()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            _modelFeatures.Add(_mocks.RunningFeatures[1].Object);

            var vm = Create();
            vm.CurrentlyEditing = vm.Items[0];
            _modelFeatures.Remove(_mocks.RunningFeatures[0].Object);
            _stripsChangeCallback();

            Assert.AreEqual(1, vm.Items.Count);
            Assert.IsNull(vm.CurrentlyEditing);
        }

        [TestMethod]
        public void MoveUp()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            var vm = Create();
            vm.MoveUp(vm.Items[0]);
            _mocks.Model.Verify(v => v.MoveUp(_mocks.RunningFeatures[0].Object), Times.Once);
        }

        [TestMethod]
        public void MoveDown()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            var vm = Create();
            vm.MoveDown(vm.Items[0]);
            _mocks.Model.Verify(v => v.MoveDown(_mocks.RunningFeatures[0].Object), Times.Once);
        }

        [TestMethod]
        public void Delete()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            var vm = Create();
            vm.Delete(vm.Items[0]);
            _mocks.Model.Verify(v => v.Delete(_mocks.RunningFeatures[0].Object), Times.Once);
        }

        [TestMethod]
        public void CurrentlyEditing_NoPreviousItem()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            var vm = Create();
            vm.CurrentlyEditing = vm.Items[0];
            Assert.IsTrue(vm.Items[0].IsEditing);
        }

        [TestMethod]
        public void CurrentlyEditing_RemoveItem()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);

            var vm = Create();
            vm.CurrentlyEditing = vm.Items[0];
            vm.CurrentlyEditing = null;

            Assert.IsFalse(vm.Items[0].IsEditing);
        }

        [TestMethod]
        public void CurrentlyEditing_ReplaceItem()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            _modelFeatures.Add(_mocks.RunningFeatures[1].Object);

            var project = Create();
            project.CurrentlyEditing = project.Items[0];
            project.CurrentlyEditing = project.Items[1];

            Assert.IsFalse(project.Items[0].IsEditing);
            Assert.IsTrue(project.Items[1].IsEditing);
        }

        [TestMethod]
        public void ShowEditingPanel_NotEditing()
        {
            Assert.IsFalse(Create().ShowEditingPanel);
        }

        [TestMethod]
        public void ShowEditingPanel_Editing()
        {
            _modelFeatures.Add(_mocks.RunningFeatures[0].Object);
            var project = Create();
            project.CurrentlyEditing = project.Items[0];
            Assert.IsTrue(project.ShowEditingPanel);
        }
    }
}
