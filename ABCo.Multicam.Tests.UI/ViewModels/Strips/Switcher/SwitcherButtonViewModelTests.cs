using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Strips.Switcher
{
    [TestClass]
    public class SwitcherButtonViewModelTests
    {
        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new SwitcherButtonViewModel(null!, Mock.Of<ISwitcherBusViewModel>(), ""));

        [TestMethod]
        public void Ctor()
        {
            var parent = Mock.Of<ISwitcherBusViewModel>();
            var serviceSource = Mock.Of<IServiceSource>();
            var vm = new SwitcherButtonViewModel(serviceSource, parent, "abc");

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual("abc", vm.Text);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }
    }
}
