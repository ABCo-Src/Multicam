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
    public class MainWindowVMTests
    {
        public record struct Mocks(Mock<IServiceSource> ServSource, Mock<IApplicationVM> App, Mock<IUIWindow> Window);
        Mocks _mocks = new();

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.App = new();
            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<IApplicationVM>()).Returns(_mocks.App.Object);
            _mocks.Window = new();
        }

        MainWindowVM Create() => new(_mocks.ServSource.Object, _mocks.Window.Object);

        [TestMethod]
        public void TitlebarHeight_Normal()
        {
            _mocks.Window.Setup(m => m.BorderRecommended).Returns(true);
            var vm = Create();
            Assert.AreEqual(38 + vm.BorderWidth, vm.TitleBarHeight);
        }

        [TestMethod]
        public void TitlebarHeight_NoBorder()
        {
            _mocks.Window.Setup(m => m.BorderRecommended).Returns(false);
            var vm = Create();
            Assert.AreEqual(38, vm.TitleBarHeight);
        }

        [TestMethod]
        public void BorderWidth_Normal()
        {
            _mocks.Window.Setup(m => m.BorderRecommended).Returns(true);
            var vm = Create();
            Assert.AreEqual(4, vm.BorderWidth);
        }

        [TestMethod]
        public void BorderWidth_NoBorder()
        {
            _mocks.Window.Setup(m => m.BorderRecommended).Returns(false);
            var vm = Create();
            Assert.AreEqual(0, vm.BorderWidth);
        }

        [TestMethod]
        public void Close()
        {
            Create().Close();
            _mocks.Window.Verify(c => c.CloseMainWindow(), Times.Once);
        }

        [TestMethod]
        public void Maximize()
        {
            Create().RequestMaximizeToggle();
            _mocks.Window.Verify(c => c.RequestMainWindowMaximizeToggle(), Times.Once);
        }

        [TestMethod]
        public void Minimize()
        {
            Create().RequestMinimize();
            _mocks.Window.Verify(c => c.RequestMainWindowMinimize(), Times.Once);
        }

        [TestMethod]
        public void ShowClose()
        {
            _mocks.Window.Setup(m => m.CloseBtnRecommended).Returns(true);
            Assert.IsTrue(Create().ShowClose);
            _mocks.Window.VerifyGet(c => c.CloseBtnRecommended, Times.Once);
        }

        [TestMethod]
        public void ShowMaximize()
        {
            _mocks.Window.Setup(m => m.CanMaximize).Returns(true);
            Assert.IsTrue(Create().ShowMaximize);
            _mocks.Window.VerifyGet(c => c.CanMaximize, Times.Once);
        }

        [TestMethod]
        public void ShowMinimize()
        {
            _mocks.Window.Setup(m => m.CanMinimize).Returns(true);
            Assert.IsTrue(Create().ShowMinimize);
            _mocks.Window.VerifyGet(c => c.CanMinimize, Times.Once);
        }

        [TestMethod]
        public void Ctor()
        {
            Assert.AreEqual(_mocks.App.Object, Create().Application);
        }

        public void IsMaximized_NotifiesRequires()
        {
            // TODO: Consistency checks
        }
    }
}
