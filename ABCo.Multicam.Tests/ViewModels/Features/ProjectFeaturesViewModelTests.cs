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
        //IServiceManager CreateDefaultFactory()
        //{
        //    IServiceManager manager = null!;
        //    var moq = new Mock<IServiceManager>();
        //    moq.Setup(i => i.CreateWithParent<IStripViewModel, IProjectStripsViewModel>(It.IsAny<IProjectStripsViewModel>()))
        //        .Callback<IProjectStripsViewModel>(parent => new StripViewModel(manager, parent));

        //    return moq.Object;
        //}

        static ProjectFeaturesViewModel CreateDefault() => new(CreateModelMockWithZeroFeatures().Object, CreateDefaultServiceSource());
        static ProjectFeaturesViewModel CreateWithCustomModel(IFeatureManager manager) => new(manager, CreateDefaultServiceSource());
        static ProjectFeaturesViewModel CreateWithCustomServSource(IServiceSource src) => new(CreateModelMockWithZeroFeatures().Object, src);
        static ProjectFeaturesViewModel CreateWithCustomModelAndServSource(IFeatureManager manager, IServiceSource src) => new(manager, src);

        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new ProjectFeaturesViewModel(CreateModelMockWithZeroFeatures().Object, null!));

        [TestMethod]
        public void Ctor_InitializesLocal()
        {
            var project = CreateDefault();
            Assert.IsNotNull(project.Items);
            Assert.IsNull(project.CurrentlyEditing);
            Assert.AreEqual(0, project.Items.Count);
        }

        [TestMethod]
        public void Ctor_InitializesEventHandler()
        {
            var model = CreateModelMockWithZeroFeatures();
            var project = CreateWithCustomModel(model.Object);
            model.Verify(m => m.SetOnFeaturesChangeForVM(It.IsAny<Action>()), Times.Once);
        }

        [TestMethod]
        public void Ctor_UpdatesItems()
        {
            List<IRunningFeature> items = new() { Mock.Of<IRunningFeature>(), Mock.Of<IRunningFeature>() };
            var model = Mock.Of<IFeatureManager>(m => m.Features == items);
            var project = CreateWithCustomModel(model);

            Assert.AreEqual(2, project.Items.Count);
            Assert.AreEqual(items[0], project.Items[0].BaseFeature);
            Assert.AreEqual(items[1], project.Items[1].BaseFeature);
        }

        [TestMethod]
        public void CreateFeature_OpensDialog()
        {
            var dialogHandler = new Mock<IUIDialogHandler>();
            var serviceSource = Mock.Of<IServiceSource>(m => m.Get<IUIDialogHandler>() == dialogHandler.Object);
            var project = CreateWithCustomServSource(serviceSource);
            project.CreateFeature();

            dialogHandler.Verify(a => a.OpenContextMenu(It.Is<ContextMenuDetails<FeatureTypes>>(d =>
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
            Action<FeatureTypes> callback = null!;

            var dialogHandler = new Mock<IUIDialogHandler>();
            dialogHandler
                .Setup(a => a.OpenContextMenu(It.IsAny<ContextMenuDetails<FeatureTypes>>()))
                .Callback<ContextMenuDetails<FeatureTypes>>((details) => callback = details.OnSelect);

            var serviceSource = Mock.Of<IServiceSource>(m => m.Get<IUIDialogHandler>() == dialogHandler.Object);
            var model = CreateModelMockWithZeroFeatures();
            var project = CreateWithCustomModelAndServSource(model.Object, serviceSource);

            project.CreateFeature();
            callback(type);
            model.Verify(m => m.CreateFeature(type), Times.Once);
        }

        [TestMethod]
        public void FeatureVMCreation_Switcher() => TestFeatureVMCreation<ISwitcherRunningFeature, ISwitcherFeatureViewModel>();

        void TestFeatureVMCreation<TStripInterface, TExpectedVMType>() 
            where TStripInterface : class, IRunningFeature
            where TExpectedVMType : class, IFeatureViewModel
        {
            var servSourceMock = new Mock<IServiceSource>();
            servSourceMock.Setup(m => m.GetWithParameter<TExpectedVMType, StripViewModelInfo>(It.IsAny<StripViewModelInfo>())).Returns(Mock.Of<TExpectedVMType>());

            IFeatureManager model = Mock.Of<IFeatureManager>(m => m.Features == new List<IRunningFeature>() { Mock.Of<TStripInterface>() });
            var project = CreateWithCustomModelAndServSource(model, servSourceMock.Object);

            Assert.IsTrue(project.Items[0].GetType().IsAssignableTo(typeof(TExpectedVMType)));
            servSourceMock.Verify(m => m.GetWithParameter<TExpectedVMType, StripViewModelInfo>(It.IsAny<StripViewModelInfo>()), Times.Once);
        }

        [TestMethod]
        public void FeaturesChange_AddToEnd()
        {
            List<IRunningFeature> stripsList = new();
            SetupFeaturesChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                var addedItem = Mock.Of<IRunningFeature>();
                stripsList.Add(addedItem);
                changeTrigger();

                Assert.AreEqual(1, project.Items.Count);
                Assert.AreEqual(addedItem, project.Items[0].BaseFeature);
            }, new());
        }

        [TestMethod]
        public void FeaturesChange_AddToStart()
        {
            var firstItemMock = Mock.Of<IRunningFeature>();
            SetupFeaturesChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                var addedItem = Mock.Of<IRunningFeature>();
                stripsList.Insert(0, addedItem);
                changeTrigger();

                Assert.AreEqual(2, project.Items.Count);
                Assert.AreEqual(addedItem, project.Items[0].BaseFeature);
                Assert.AreEqual(firstItemMock, project.Items[1].BaseFeature);
            }, new() { firstItemMock });
        }

        [TestMethod]
        public void FeaturesChange_RemoveFromStart()
        {
            var firstItemMock = Mock.Of<IRunningFeature>();
            var secondItemMock = Mock.Of<IRunningFeature>();
            SetupFeaturesChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                stripsList.Remove(firstItemMock);
                changeTrigger();

                Assert.AreEqual(1, project.Items.Count);
                Assert.AreEqual(secondItemMock, project.Items[0].BaseFeature);
            }, new() { firstItemMock, secondItemMock });
        }

        [TestMethod]
        public void FeaturesChange_Remove_Editing()
        {
            var firstItemMock = Mock.Of<IRunningFeature>();
            SetupFeaturesChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                project.CurrentlyEditing = project.Items[0];

                stripsList.Remove(firstItemMock);
                changeTrigger();

                Assert.AreEqual(1, project.Items.Count);
                Assert.IsNull(project.CurrentlyEditing);
            }, new() { firstItemMock, Mock.Of<IRunningFeature>() });
        }

        private void SetupFeaturesChangeMockAndVM(Action<ProjectFeaturesViewModel, Action, List<IRunningFeature>> testCode, List<IRunningFeature> stripsList)
        {
            Action changeTrigger = null!;
            var model = new Mock<IFeatureManager>();
            model.Setup(e => e.SetOnFeaturesChangeForVM(It.IsAny<Action>())).Callback<Action>(a => changeTrigger = a);
            model.SetupGet(e => e.Features).Returns(() => stripsList);

            var project = CreateWithCustomModel(model.Object);
            testCode(project, changeTrigger, stripsList);
        }

        [TestMethod]
        public void MoveUp()
        {
            var model = CreateModelMockWithOneStrip(out var initialStrip);
            var project = CreateWithCustomModel(model.Object);
            project.MoveUp(project.Items[0]);
            model.Verify(v => v.MoveUp(initialStrip), Times.Once);
        }

        [TestMethod]
        public void MoveDown()
        {
            var model = CreateModelMockWithOneStrip(out var initialStrip);
            var project = CreateWithCustomModel(model.Object);
            project.MoveDown(project.Items[0]);
            model.Verify(v => v.MoveDown(initialStrip), Times.Once);
        }

        [TestMethod]
        public void Delete()
        {
            var model = CreateModelMockWithOneStrip(out var initialStrip);
            var project = CreateWithCustomModel(model.Object);
            project.Delete(project.Items[0]);
            model.Verify(v => v.Delete(initialStrip), Times.Once);
        }

        [TestMethod]
        public void CurrentlyEditing_NoPreviousItem()
        {
            var project = CreateWithCustomModel(CreateModelMockWithOneStrip(out _).Object);
            project.CurrentlyEditing = project.Items[0];
            Assert.IsTrue(project.Items[0].IsEditing);
        }

        [TestMethod]
        public void CurrentlyEditing_RemoveItem()
        {
            var project = CreateWithCustomModel(CreateModelMockWithOneStrip(out _).Object);

            project.CurrentlyEditing = project.Items[0];
            project.CurrentlyEditing = null;

            Assert.IsFalse(project.Items[0].IsEditing);
        }

        [TestMethod]
        public void CurrentlyEditing_ReplaceItem()
        {
            var model = Mock.Of<IFeatureManager>(m => m.Features == new List<IRunningFeature>() { Mock.Of<IRunningFeature>(), Mock.Of<IRunningFeature>() });
            var project = CreateWithCustomModel(model);

            project.CurrentlyEditing = project.Items[0];
            project.CurrentlyEditing = project.Items[1];

            Assert.IsFalse(project.Items[0].IsEditing);
            Assert.IsTrue(project.Items[1].IsEditing);

            Assert.IsFalse(project.Items[0].IsEditing);
        }

        [TestMethod]
        public void ShowEditingPanel_NotEditing()
        {
            var project = CreateWithCustomModel(CreateModelMockWithZeroFeatures().Object);
            Assert.IsFalse(project.ShowEditingPanel);
        }

        [TestMethod]
        public void ShowEditingPanel_Editing()
        {
            var project = CreateWithCustomModel(CreateModelMockWithOneStrip(out _).Object);
            project.CurrentlyEditing = project.Items[0];
            Assert.IsTrue(project.ShowEditingPanel);
        }

        // HELPERS:
        static Mock<IFeatureManager> CreateModelMockWithZeroFeatures()
        {
            var model = new Mock<IFeatureManager>();
            model.SetReturnsDefault<IReadOnlyList<IRunningFeature>>(new List<IRunningFeature>());
            return model;
        }

        static Mock<IFeatureManager> CreateModelMockWithOneStrip(out IRunningFeature strip)
        {
            strip = Mock.Of<IRunningFeature>();
            var model = new Mock<IFeatureManager>();
            model.SetReturnsDefault<IReadOnlyList<IRunningFeature>>(new List<IRunningFeature>() { strip  });
            return model;
        }

        static IServiceSource CreateDefaultServiceSource() => Mock.Of<IServiceSource>();
    }
}
