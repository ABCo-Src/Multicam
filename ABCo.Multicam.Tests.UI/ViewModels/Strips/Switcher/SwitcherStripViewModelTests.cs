using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Strips;
using ABCo.Multicam.UI.ViewModels.Strips.BusGroup;
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
            Assert.IsNotNull(vm.ButtonColumns);
        }

        [TestMethod]
        public void ContentView()
        {
            var vm = new SwitcherStripViewModel(Mock.Of<IServiceSource>(), Mock.Of<IProjectStripsViewModel>());
            Assert.AreEqual(StripViewType.Switcher, vm.ContentView);
        }
    }
}
