using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Structures;
using ABCo.Multicam.Core.Switchers;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Strips;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Strips
{
    [TestClass]
    public class ProjectStripsViewModelTests
    {
        //IServiceManager CreateDefaultFactory()
        //{
        //    IServiceManager manager = null!;
        //    var moq = new Mock<IServiceManager>();
        //    moq.Setup(i => i.CreateWithParent<IStripViewModel, IProjectStripsViewModel>(It.IsAny<IProjectStripsViewModel>()))
        //        .Callback<IProjectStripsViewModel>(parent => new StripViewModel(manager, parent));

        //    return moq.Object;
        //}

        static ProjectStripsViewModel CreateDefault() => new(CreateModelMockWithZeroStrips().Object, CreateDefaultServiceSource());
        static ProjectStripsViewModel CreateWithCustomModel(IStripManager manager) => new(manager, CreateDefaultServiceSource());
        static ProjectStripsViewModel CreateWithCustomServSource(IServiceSource src) => new(CreateModelMockWithZeroStrips().Object, src);
        static ProjectStripsViewModel CreateWithCustomModelAndServSource(IStripManager manager, IServiceSource src) => new(manager, src);

        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new ProjectStripsViewModel(CreateModelMockWithZeroStrips().Object, null!));

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
            var model = CreateModelMockWithZeroStrips();
            var project = CreateWithCustomModel(model.Object);
            model.Verify(m => m.SetStripsChangeForVM(It.IsAny<Action>()), Times.Once);
        }

        [TestMethod]
        public void Ctor_UpdatesItems()
        {
            List<IRunningStrip> items = new() { Mock.Of<IRunningStrip>(), Mock.Of<IRunningStrip>() };
            var model = Mock.Of<IStripManager>(m => m.Strips == items);
            var project = CreateWithCustomModel(model);

            Assert.AreEqual(2, project.Items.Count);
            Assert.AreEqual(items[0], project.Items[0].BaseStrip);
            Assert.AreEqual(items[1], project.Items[1].BaseStrip);
        }

        [TestMethod]
        public void CreateStrip_OpensDialog()
        {
            var dialogHandler = new Mock<IUIDialogHandler>();
            var serviceSource = Mock.Of<IServiceSource>(m => m.Get<IUIDialogHandler>() == dialogHandler.Object);
            var project = CreateWithCustomServSource(serviceSource);
            project.CreateStrip();

            dialogHandler.Verify(a => a.OpenContextMenu(It.Is<ContextMenuDetails<StripTypes>>(d =>
                d.Title == "Choose Type" &&
                d.OnSelect != null &&
                d.OnCancel == null &&
                d.Items.SequenceEqual(new ContextMenuItem<StripTypes>[]
                    {
                        new("Switcher", StripTypes.Switcher),
                        new("Tally", StripTypes.Tally)
                    })
                ))
            );
        }

        [TestMethod]
        [DataRow(StripTypes.Switcher)]
        [DataRow(StripTypes.Tally)]
        public void CreateStrip_OnChoose(StripTypes type)
        {
            Action<StripTypes> callback = null!;

            var dialogHandler = new Mock<IUIDialogHandler>();
            dialogHandler
                .Setup(a => a.OpenContextMenu(It.IsAny<ContextMenuDetails<StripTypes>>()))
                .Callback<ContextMenuDetails<StripTypes>>((details) => callback = details.OnSelect);

            var serviceSource = Mock.Of<IServiceSource>(m => m.Get<IUIDialogHandler>() == dialogHandler.Object);
            var model = CreateModelMockWithZeroStrips();
            var project = CreateWithCustomModelAndServSource(model.Object, serviceSource);

            project.CreateStrip();
            callback(type);
            model.Verify(m => m.CreateStrip(type), Times.Once);
        }

        [TestMethod]
        public void StripVMCreation_Unsupported() => TestStripVMCreation<IRunningStrip, UnsupportedStripViewModel>();

        [TestMethod]
        public void StripVMCreation_Switcher() => TestStripVMCreation<ISwitcherRunningStrip, SwitcherStripViewModel>();

        void TestStripVMCreation<TStripInterface, TExpectedVMType>() where TStripInterface : class, IRunningStrip
        {
            IStripManager model = Mock.Of<IStripManager>(m => m.Strips == new List<IRunningStrip>() { Mock.Of<TStripInterface>() });
            var project = CreateWithCustomModel(model);
            Assert.IsInstanceOfType(project.Items[0], typeof(TExpectedVMType));
        }

        [TestMethod]
        public void StripsChange_AddToEnd()
        {
            List<IRunningStrip> stripsList = new();
            SetupStripsChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                var addedItem = Mock.Of<IRunningStrip>();
                stripsList.Add(addedItem);
                changeTrigger();

                Assert.AreEqual(1, project.Items.Count);
                Assert.AreEqual(addedItem, project.Items[0].BaseStrip);
            }, new());
        }

        [TestMethod]
        public void StripsChange_AddToStart()
        {
            var firstItemMock = Mock.Of<IRunningStrip>();
            SetupStripsChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                var addedItem = Mock.Of<IRunningStrip>();
                stripsList.Insert(0, addedItem);
                changeTrigger();

                Assert.AreEqual(2, project.Items.Count);
                Assert.AreEqual(addedItem, project.Items[0].BaseStrip);
                Assert.AreEqual(firstItemMock, project.Items[1].BaseStrip);
            }, new() { firstItemMock });
        }

        [TestMethod]
        public void StripsChange_RemoveFromStart()
        {
            var firstItemMock = Mock.Of<IRunningStrip>();
            var secondItemMock = Mock.Of<IRunningStrip>();
            SetupStripsChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                stripsList.Remove(firstItemMock);
                changeTrigger();

                Assert.AreEqual(1, project.Items.Count);
                Assert.AreEqual(secondItemMock, project.Items[0].BaseStrip);
            }, new() { firstItemMock, secondItemMock });
        }

        [TestMethod]
        public void StripsChange_Remove_Editing()
        {
            var firstItemMock = Mock.Of<IRunningStrip>();
            SetupStripsChangeMockAndVM((project, changeTrigger, stripsList) =>
            {
                project.CurrentlyEditing = project.Items[0];

                stripsList.Remove(firstItemMock);
                changeTrigger();

                Assert.AreEqual(1, project.Items.Count);
                Assert.IsNull(project.CurrentlyEditing);
            }, new() { firstItemMock, Mock.Of<IRunningStrip>() });
        }

        private void SetupStripsChangeMockAndVM(Action<ProjectStripsViewModel, Action, List<IRunningStrip>> testCode, List<IRunningStrip> stripsList)
        {
            Action changeTrigger = null!;
            var model = new Mock<IStripManager>();
            model.Setup(e => e.SetStripsChangeForVM(It.IsAny<Action>())).Callback<Action>(a => changeTrigger = a);
            model.SetupGet(e => e.Strips).Returns(() => stripsList);

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
            var model = Mock.Of<IStripManager>(m => m.Strips == new List<IRunningStrip>() { Mock.Of<IRunningStrip>(), Mock.Of<IRunningStrip>() });
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
            var project = CreateWithCustomModel(CreateModelMockWithZeroStrips().Object);
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
        static Mock<IStripManager> CreateModelMockWithZeroStrips()
        {
            var model = new Mock<IStripManager>();
            model.SetReturnsDefault<IReadOnlyList<IRunningStrip>>(new List<IRunningStrip>());
            return model;
        }

        static Mock<IStripManager> CreateModelMockWithOneStrip(out IRunningStrip strip)
        {
            strip = Mock.Of<IRunningStrip>();
            var model = new Mock<IStripManager>();
            model.SetReturnsDefault<IReadOnlyList<IRunningStrip>>(new List<IRunningStrip>() { strip  });
            return model;
        }

        static IServiceSource CreateDefaultServiceSource() => Mock.Of<IServiceSource>();
    }
}
