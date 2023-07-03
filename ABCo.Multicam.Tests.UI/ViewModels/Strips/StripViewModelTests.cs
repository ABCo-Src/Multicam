using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
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
    public class StripViewModelTests
    {
        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new StripViewModel(null!, Mock.Of<IProjectStripsViewModel>()));

        [TestMethod]
        public void Ctor()
        {
            var parent = Mock.Of<IProjectStripsViewModel>();
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), parent);

            Assert.AreEqual("New Strip", vm.StripTitle);
            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(false, vm.IsEditing);
        }

        [TestMethod]
        public void ToggleEdit_NotEditing()
        {
            var parent = new Mock<IProjectStripsViewModel>();
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), parent.Object);

            vm.ToggleEdit();

            parent.VerifySet(i => i.CurrentlyEditing = vm);
        }

        [TestMethod]
        public void ToggleEdit_Editing()
        {
            var parent = new Mock<IProjectStripsViewModel>();
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), parent.Object);

            vm.IsEditing = true;
            vm.ToggleEdit();

            parent.VerifySet(i => i.CurrentlyEditing = null);
        }

        [TestMethod]
        public void EditBtnText_NotEditing()
        {
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), Mock.Of<IProjectStripsViewModel>());
            vm.IsEditing = false;
            Assert.AreEqual("Edit", vm.EditBtnText);
        }

        [TestMethod]
        public void EditBtnText_Editing()
        {
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), Mock.Of<IProjectStripsViewModel>());
            vm.IsEditing = true;
            Assert.AreEqual("Finish", vm.EditBtnText);
        }

        [TestMethod]
        public void MoveDown()
        {
            var parent = new Mock<IProjectStripsViewModel>();
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), parent.Object);
            vm.MoveDown();
            parent.Verify(i => i.MoveDown(vm));
        }

        [TestMethod]
        public void MoveUp()
        {
            var parent = new Mock<IProjectStripsViewModel>();
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), parent.Object);
            vm.MoveUp();
            parent.Verify(i => i.MoveUp(vm));
        }

        [TestMethod]
        public void Delete()
        {
            var parent = new Mock<IProjectStripsViewModel>();
            var vm = new StripViewModel(Mock.Of<IServiceSource>(), parent.Object);
            vm.Delete();
            parent.Verify(i => i.Delete(vm));
        }

        [TestMethod]
        public void IsEditing_UpdatesEditBtnText()
        {
            // TODO: Create viewmodel consistency tests
        }
    }
}
