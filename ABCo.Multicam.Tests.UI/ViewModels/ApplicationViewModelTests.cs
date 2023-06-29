using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels
{
    [TestClass]
    public class ApplicationViewModelTests
    {
        [TestMethod]
        public void ApplicationVM_CtorAssignsCorrectly()
        {
            var projVmMoq = Mock.Of<IProjectViewModel>();
            var vm = new ApplicationViewModel(projVmMoq);
            Assert.AreEqual(projVmMoq, vm.Project);
        }
    }
}
