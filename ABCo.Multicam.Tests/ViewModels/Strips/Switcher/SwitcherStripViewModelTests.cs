using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Switchers.Types;
using ABCo.Multicam.UI.Enumerations;
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
    public class SwitcherStripViewModelTests
    {
        class Dummy : SwitcherStripViewModel
        {
            public IServiceSource Source => _serviceSource;
            public Dummy(IServiceSource serviceSource, IProjectStripsViewModel parent) : base(serviceSource, parent) { }
        }

        [TestMethod]
        public void Ctor()
        {
            var parent = Mock.Of<IProjectStripsViewModel>();
            var serviceSource = Mock.Of<IServiceSource>();
            var vm = new Dummy(serviceSource, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(serviceSource, vm.Source);
            Assert.IsNotNull(vm.MixBlocks);
        }

        [TestMethod]
        public void Ctor_StartsWithDummySwitcher()
        {            
            var dummySwitcher = Mock.Of<IDummySwitcher>();
            var serviceSource = new Mock<IServiceSource>();
            serviceSource.Setup(s => s.Get<IDummySwitcher>()).Returns(dummySwitcher);

            var vm = new Dummy(serviceSource.Object, Mock.Of<IProjectStripsViewModel>());

            serviceSource.Verify(s => s.Get<IDummySwitcher>(), Times.Once);
        }

        [TestMethod]
        public void ContentView()
        {
            var vm = new SwitcherStripViewModel(Mock.Of<IServiceSource>(), Mock.Of<IProjectStripsViewModel>());
            Assert.AreEqual(StripViewType.Switcher, vm.ContentView);
        }
    }
}
