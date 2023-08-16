using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
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
        public void Ctor_Normal()
        {
            var vm = new ApplicationViewModel(Mock.Of<IServiceSource>(m => m.Get<IProjectViewModel>() == Mock.Of<IProjectViewModel>()));
            Assert.IsNotNull(vm.Project);
        }
    }
}
