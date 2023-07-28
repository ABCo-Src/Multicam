using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.ViewModels.Features.Switcher
{
    [TestClass]
    public class SwitcherBusInputViewModelTests
    {
        public SwitcherBusInputViewModel CreateDefault() => new SwitcherBusInputViewModel(new SwitcherBusInput(), false, Mock.Of<IServiceSource>(), Mock.Of<ISwitcherMixBlockViewModel>());
        public SwitcherBusInputViewModel CreateWithModel(SwitcherBusInput input, bool isProgram) =>
            new SwitcherBusInputViewModel(input, isProgram, Mock.Of<IServiceSource>(), Mock.Of<ISwitcherMixBlockViewModel>());
        public SwitcherBusInputViewModel CreateWithParent(SwitcherBusInput input, bool isProgram, ISwitcherMixBlockViewModel parent) => 
            new SwitcherBusInputViewModel(input, isProgram, Mock.Of<IServiceSource>(), parent);

        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new SwitcherBusInputViewModel(new SwitcherBusInput(), false, null!, Mock.Of<ISwitcherMixBlockViewModel>()));

        [TestMethod]
        public void Ctor_Program()
        {
            var parent = Mock.Of<ISwitcherMixBlockViewModel>();
            var baseModel = new SwitcherBusInput(1, "abc");
            var vm = CreateWithParent(baseModel, true, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.IsTrue(vm.IsProgram);
            Assert.AreEqual(baseModel, vm.Base);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        public void Ctor_Preview()
        {
            var parent = Mock.Of<ISwitcherMixBlockViewModel>();
            var baseModel = new SwitcherBusInput(4, "def");
            var vm = CreateWithParent(baseModel, false, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.IsFalse(vm.IsProgram);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
            Assert.AreEqual(baseModel, vm.Base);            
        }

        [TestMethod]
        public void Text()
        {
            var vm = CreateWithModel(new SwitcherBusInput(1, "Cam1"), true);
            Assert.AreEqual("Cam1", vm.Text);
        }
    }
}
