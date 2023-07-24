using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Strips;
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
    public class SwitcherMixBlockViewModelTests
    {
        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => 
        new SwitcherMixBlockViewModel(new SwitcherMixBlock(), null!, Mock.Of<ISwitcherStripViewModel>()));

        [TestMethod]
        public void Ctor()
        {
            var model = new SwitcherMixBlock();
            var parent = Mock.Of<ISwitcherStripViewModel>();
            SwitcherMixBlockViewModel vm = CreateWithParent(model, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(model, vm.BaseBlock);
            Assert.IsNotNull(vm.ProgramBus);
            Assert.IsNotNull(vm.PreviewBus);
            Assert.IsNotNull(vm.CutButton);
            Assert.IsNotNull(vm.AutoButton);

            Assert.AreEqual("Cut", vm.CutButton.Text);
            Assert.AreEqual("Auto", vm.AutoButton.Text);
        }

        private static SwitcherMixBlockViewModel CreateDefault(SwitcherMixBlock model) => new SwitcherMixBlockViewModel(model, Mock.Of<IServiceSource>(), Mock.Of<ISwitcherStripViewModel>());
        private static SwitcherMixBlockViewModel CreateWithParent(SwitcherMixBlock model, ISwitcherStripViewModel parent) => new SwitcherMixBlockViewModel(model, Mock.Of<IServiceSource>(), parent);
    }
}
