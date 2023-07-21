﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Strips;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        IServiceSource CreateDefaultServiceSource() => Mock.Of<IServiceSource>();

        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new ProjectStripsViewModel(Mock.Of<IStripManager>(), null!));

        [TestMethod]
        public void Ctor_InitializedOC()
        {
            var project = new ProjectStripsViewModel(Mock.Of<IStripManager>(), CreateDefaultServiceSource());
            Assert.IsNotNull(project.Items);
            Assert.IsNull(project.CurrentlyEditing);
            Assert.AreEqual(0, project.Items.Count);
        }

        [TestMethod]
        public void CreateStrip()
        {
            var model = new Mock<IStripManager>();
            var project = new ProjectStripsViewModel(model.Object, CreateDefaultServiceSource());
            project.CreateStrip();
            model.Verify(v => v.CreateStrip(), Times.Once);
        }

        //[TestMethod]
        //public void MoveUp()
        //{
        //    var model = new Mock<IStripManager>();
        //    var vm = new 
        //    var project = new ProjectStripsViewModel(model.Object, CreateDefaultServiceSource());
        //    project.MoveUp();
        //    model.Verify(v => v.MoveUp(), Times.Once);
        //}

        //[TestMethod]
        //public void CreateStrip_Normal_StripHasParent()
        //{
        //    var serviceSource = CreateDefaultServiceSource();
        //    var project = new ProjectStripsViewModel(serviceSource);
        //    project.AddStrip();

        //    Assert.AreEqual(project, project.Items[0].Parent);
        //}

        //[TestMethod]
        //public void Delete()
        //{
        //    var serviceSource = CreateDefaultServiceSource();
        //    var project = new ProjectStripsViewModel(serviceSource);
        //    project.CreateStrip();
        //    project.CreateStrip();
        //    project.CreateStrip();
        //    var unmoving1 = project.Items[0];
        //    var movingStrip = project.Items[1];
        //    var unmoving2 = project.Items[2];

        //    project.Delete(movingStrip);

        //    Assert.AreEqual(unmoving1, project.Items[0]);
        //    Assert.AreEqual(unmoving2, project.Items[1]);
        //}

        //[TestMethod]
        //public void Delete_Editing()
        //{
        //    var serviceSource = CreateDefaultServiceSource();
        //    var project = new ProjectStripsViewModel(serviceSource);
        //    project.CreateStrip();
        //    var newStrip = project.Items[0];
        //    project.CurrentlyEditing = newStrip;

        //    project.Delete(newStrip);

        //    Assert.IsNull(project.CurrentlyEditing);
        //    Assert.IsFalse(newStrip.IsEditing);
        //}

        //[TestMethod]
        //public void Delete_OtherEditing()
        //{
        //    var serviceSource = CreateDefaultServiceSource();
        //    var project = new ProjectStripsViewModel(serviceSource);
        //    project.CreateStrip();
        //    project.CreateStrip();
        //    var newStrip = project.Items[0];
        //    project.CurrentlyEditing = project.Items[1];

        //    project.Delete(newStrip);

        //    Assert.AreEqual(project.Items[0], project.CurrentlyEditing);
        //}

        //[TestMethod]
        //public void CreateStrip_Normal_UsesFactory()
        //{
        //    StripViewModel? stripVM = null;
        //    stripVM = new StripViewModel(CreateDefaultFactory());

        //    var factoryFactory = new Mock<IServiceProvider>(MockBehavior.Strict);
        //    factoryFactory.Setup(x => x.Create<IStripViewModel>()).Returns(() => stripVM!);
        //    factoryFactory.Setup(x => x.Create<StripViewModel>()).Throws(new Exception("Should ask for interface, not concrete."));

        //    var project = new ProjectStripsViewModel(factoryFactory.Object);

        //    project.AddStrip();
        //    factoryFactory.Verify(x => x.Create<IStripViewModel>(), Times.Once);
        //}

        //[TestMethod]
        //public void CurrentlyEditing_NoPreviousItem()
        //{
        //    var project = new ProjectStripsViewModel(CreateDefaultServiceSource());
        //    project.CreateStrip();

        //    project.CurrentlyEditing = project.Items[0];
        //    Assert.IsTrue(project.Items[0].IsEditing);
        //}

        //[TestMethod]
        //public void CurrentlyEditing_RemoveItem()
        //{
        //    var project = new ProjectStripsViewModel(CreateDefaultServiceSource());
        //    project.CreateStrip();

        //    project.CurrentlyEditing = project.Items[0];
        //    project.CurrentlyEditing = null;

        //    Assert.IsFalse(project.Items[0].IsEditing);
        //}

        //// Potentially redundant test?
        //[TestMethod]
        //public void CurrentlyEditing_ReplaceItem()
        //{
        //    var project = new ProjectStripsViewModel(CreateDefaultServiceSource());
        //    project.CreateStrip();
        //    project.CreateStrip();

        //    project.CurrentlyEditing = project.Items[0];
        //    project.CurrentlyEditing = project.Items[1];

        //    Assert.IsFalse(project.Items[0].IsEditing);
        //    Assert.IsTrue(project.Items[1].IsEditing);
        //}

        //[TestMethod]
        //public void ShowEditingPanel_NotEditing()
        //{
        //    var project = new ProjectStripsViewModel(CreateDefaultServiceSource());
        //    Assert.IsFalse(project.ShowEditingPanel);
        //}

        //[TestMethod]
        //public void ShowEditingPanel_Editing()
        //{
        //    var project = new ProjectStripsViewModel(CreateDefaultServiceSource());
        //    project.CreateStrip();
        //    project.CurrentlyEditing = project.Items[0];
        //    Assert.IsTrue(project.ShowEditingPanel);
        //}

        //[TestMethod]
        //public void ShowEditingPanel_ChangesWithCurrentlyEditing()
        //{
        //    // TODO: Consistency check
        //}
    }
}
