using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips.Switchers;
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
        public SwitcherButtonViewModel CreateDefault() => new SwitcherButtonViewModel(Mock.Of<IServiceSource>(), Mock.Of<ISwitcherMixBlockViewModel>());
        public SwitcherButtonViewModel CreateWithParent(ISwitcherMixBlockViewModel parent) => new SwitcherButtonViewModel(Mock.Of<IServiceSource>(), parent);
        public SwitcherButtonViewModel CreateBusInputWithParent(SwitcherBusInput input, bool isProgram, ISwitcherMixBlockViewModel parent) => new SwitcherButtonViewModel(input, isProgram, Mock.Of<IServiceSource>(), parent);
        public SwitcherButtonViewModel CreateGeneralWithParent(SwitcherButtonViewModel.Action action, ISwitcherMixBlockViewModel parent) => new SwitcherButtonViewModel(action, Mock.Of<IServiceSource>(), parent);

        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new SwitcherButtonViewModel(null!, Mock.Of<ISwitcherMixBlockViewModel>()));

        [TestMethod]
        public void Ctor()
        {
            var parent = Mock.Of<ISwitcherMixBlockViewModel>();
            var vm = CreateWithParent(parent);
            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        public void Ctor_ProgramBusInput()
        {
            var parent = Mock.Of<ISwitcherMixBlockViewModel>();
            var vm = CreateBusInputWithParent(new SwitcherBusInput(1, "abc"), true, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
            Assert.AreEqual(SwitcherButtonViewModel.Action.ProgramSwitch, vm.ClickAction);
            Assert.AreEqual(1, vm.Id);
            Assert.AreEqual("abc", vm.Text);
        }

        [TestMethod]
        public void Ctor_PreviewBusInput()
        {
            var parent = Mock.Of<ISwitcherMixBlockViewModel>();
            var vm = CreateBusInputWithParent(new SwitcherBusInput(3, "abc"), false, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
            Assert.AreEqual(SwitcherButtonViewModel.Action.PreviewSwitch, vm.ClickAction);
            Assert.AreEqual(3, vm.Id);
            Assert.AreEqual("abc", vm.Text);
        }

        [TestMethod]
        public void Ctor_Other()
        {
            var parent = Mock.Of<ISwitcherMixBlockViewModel>();
            var vm = CreateBusInputWithParent(new SwitcherBusInput(3, "abc"), false, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
            Assert.AreEqual(SwitcherButtonViewModel.Action.PreviewSwitch, vm.ClickAction);
            Assert.AreEqual(3, vm.Id);
            Assert.AreEqual("abc", vm.Text);
        }
    }
}
