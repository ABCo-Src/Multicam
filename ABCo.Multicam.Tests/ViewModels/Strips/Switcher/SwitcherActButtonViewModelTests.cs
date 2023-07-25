using ABCo.Multicam.Core;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.ViewModels.Strips.Switcher
{
    [TestClass]
    public class SwitcherActButtonViewModelTests
    {
        public SwitcherActButtonViewModel CreateDefault(SwitcherActButtonViewModel.Type act) => 
            new SwitcherActButtonViewModel(act, Mock.Of<IServiceSource>(), Mock.Of<ISwitcherMixBlockViewModel>());
        public SwitcherActButtonViewModel CreateWithParent(SwitcherActButtonViewModel.Type act, ISwitcherMixBlockViewModel parent) => 
            new SwitcherActButtonViewModel(act, Mock.Of<IServiceSource>(), parent);

        [TestMethod]
        [DataRow(SwitcherActButtonViewModel.Type.Cut, "Cut")]
        [DataRow(SwitcherActButtonViewModel.Type.Auto, "Auto")]
        public void Ctor(SwitcherActButtonViewModel.Type action, string expectedText)
        {
            var parent = Mock.Of<ISwitcherMixBlockViewModel>();
            var vm = CreateWithParent(action, parent);
            Assert.AreEqual(expectedText, vm.Text);
            Assert.AreEqual(action, vm.Action);
            Assert.AreEqual(parent, vm.Parent);
        }
    }
}
