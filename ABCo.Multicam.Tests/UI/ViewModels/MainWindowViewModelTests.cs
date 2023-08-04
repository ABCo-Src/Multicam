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
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void TitlebarHeight_Normal()
        {
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), Mock.Of<IUIWindow>(m => m.BorderRecommended == true));
            Assert.AreEqual(38 + vm.BorderWidth, vm.TitleBarHeight);
        }

        [TestMethod]
        public void TitlebarHeight_NoBorder()
        {
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), Mock.Of<IUIWindow>(m => m.BorderRecommended == false));
            Assert.AreEqual(38, vm.TitleBarHeight);
        }

        [TestMethod]
        public void BorderWidth_Normal()
        {
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), Mock.Of<IUIWindow>(m => m.BorderRecommended == true));
            Assert.AreEqual(4, vm.BorderWidth);
        }

        [TestMethod]
        public void BorderWidth_NoBorder()
        {
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), Mock.Of<IUIWindow>(m => m.BorderRecommended == false));
            Assert.AreEqual(0, vm.BorderWidth);
        }

        [TestMethod]
        public void Close()
        {
            var windowMock = new Mock<IUIWindow>();
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), windowMock.Object);
            vm.Close();
            windowMock.Verify(c => c.CloseMainWindow(), Times.Once);
        }

        [TestMethod]
        public void Maximize()
        {
            var windowMock = new Mock<IUIWindow>();
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), windowMock.Object);
            vm.RequestMaximizeToggle();
            windowMock.Verify(c => c.RequestMainWindowMaximizeToggle(), Times.Once);
        }

        [TestMethod]
        public void Minimize()
        {
            var windowMock = new Mock<IUIWindow>();
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), windowMock.Object);
            vm.RequestMinimize();
            windowMock.Verify(c => c.RequestMainWindowMinimize(), Times.Once);
        }

        [TestMethod]
        public void ShowClose()
        {
            var windowMock = new Mock<IUIWindow>();
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), windowMock.Object);
            _ = vm.ShowClose;
            windowMock.VerifyGet(c => c.CloseBtnRecommended, Times.Once);
        }

        [TestMethod]
        public void ShowMaximize()
        {
            var windowMock = new Mock<IUIWindow>();
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), windowMock.Object);
            _ = vm.ShowMaximize;
            windowMock.VerifyGet(c => c.CanMaximize, Times.Once);
        }

        [TestMethod]
        public void ShowMinimize()
        {
            var windowMock = new Mock<IUIWindow>();
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), windowMock.Object);
            _ = vm.ShowMinimize;
            windowMock.VerifyGet(c => c.CanMinimize, Times.Once);
        }

        [TestMethod]
        public void Ctor()
        {
            var vm = new MainWindowViewModel(Mock.Of<IServiceSource>(), Mock.Of<IUIWindow>());
            Assert.IsNotNull(vm.Application);
        }

        public void IsMaximized_NotifiesRequires()
        {
            // TODO: Consistency checks
        }

        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new MainWindowViewModel(null!, Mock.Of<IUIWindow>()));
    }
}
