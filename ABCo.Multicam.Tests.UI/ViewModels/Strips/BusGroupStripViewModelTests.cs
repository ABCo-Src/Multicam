using ABCo.Multicam.Core;
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
    public class BusGroupStripViewModelTests
    {
        class Dummy : BusGroupStripViewModel
        {
            protected IServiceSource Source => _serviceSource;
            public Dummy(IServiceSource serviceSource, IProjectStripsViewModel parent) : base(serviceSource, parent) { }
        }

        [TestMethod]
        public void Ctor()
        {
            var parent = Mock.Of<IProjectStripsViewModel>();
            var serviceSource = Mock.Of<IServiceSource>();
            var vm = new DummyStripViewModel(serviceSource, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(serviceSource, vm.Source);
        }
    }
}
