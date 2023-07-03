using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Strips;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels
{
    [TestClass]
    public class ProjectViewModelTests
    {
        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new ProjectViewModel(null!));

        [TestMethod]
        public void Ctor_Normal()
        {
            var vm = new ProjectViewModel(Mock.Of<IServiceSource>());
            Assert.IsNotNull(vm.Strips);
        }
    }
}
