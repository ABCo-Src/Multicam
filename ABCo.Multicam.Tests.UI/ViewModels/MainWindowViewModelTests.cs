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
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void TitlebarHeight_Normal()
        {
            var vm = new MainWindowViewModel(Mock.Of<IUIPlatformWindowCapabilities>(m => m.BorderRecommended == true), Mock.Of<IApplicationViewModel>());
            Assert.AreEqual(38 + vm.BorderWidth, vm.TitleBarHeight);
        }

        [TestMethod]
        public void TitlebarHeight_NoBorder()
        {
            var vm = new MainWindowViewModel(Mock.Of<IUIPlatformWindowCapabilities>(m => m.BorderRecommended == false), Mock.Of<IApplicationViewModel>());
            Assert.AreEqual(38, vm.TitleBarHeight);
        }

        [TestMethod]
        public void BorderWidth_Normal()
        {
            var vm = new MainWindowViewModel(Mock.Of<IUIPlatformWindowCapabilities>(m => m.BorderRecommended == true), Mock.Of<IApplicationViewModel>());
            Assert.AreEqual(2, vm.BorderWidth);
        }

        [TestMethod]
        public void BorderWidth_NoBorder()
        {
            var vm = new MainWindowViewModel(Mock.Of<IUIPlatformWindowCapabilities>(m => m.BorderRecommended == false), Mock.Of<IApplicationViewModel>());
            Assert.AreEqual(0, vm.BorderWidth);
        }

        [TestMethod]
        public void ProjectVM_CtorAssignsCorrectly()
        {
            var appVmMoq = Mock.Of<IApplicationViewModel>();
            var vm = new MainWindowViewModel(Mock.Of<IUIPlatformWindowCapabilities>(), appVmMoq);
            Assert.AreEqual(appVmMoq, vm.Application);
        }
    }
}
