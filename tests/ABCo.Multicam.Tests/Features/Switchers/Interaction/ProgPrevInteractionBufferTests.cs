//using ABCo.Multicam.Core.Features.Switchers;
//using ABCo.Multicam.Core.Features.Switchers.Interaction;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ABCo.Multicam.Tests.Features.Switchers.Interaction
//{
//    [TestClass]
//    public class ProgPrevInteractionBufferTests
//    {
//        public record struct Mocks(
//            Mock<ISwitcher> Switcher
//        );

//        SwitcherMixBlock _mixBlock = new();
//        int _index = 7;
//        Mocks _mocks = new();

//        ProgPrevInteractionBuffer Create() => new(_mocks.Switcher.Object, _mixBlock, _index);

//        [TestInitialize]
//        public void InitMocks()
//        {
//            _mixBlock = new();
//            _index = 7;
//            _mocks.Switcher = new();
//            _mocks.Switcher.Setup(m => m.ReceiveValue(7, 0)).Returns(2);
//            _mocks.Switcher.Setup(m => m.ReceiveValue(7, 1)).Returns(4);
//        }

//        [TestMethod]
//        public void GetValue_Program()
//        {
//            var buffer = Create();

//            Assert.AreEqual(2, buffer.GetValue(0));
//            Assert.AreEqual(2, buffer.GetValue(0));

//            // Verify it was received once
//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 0), Times.Once);
//        }

//        [TestMethod]
//        public void GetValue_Preview()
//        {
//            var buffer = Create();

//            Assert.AreEqual(4, buffer.GetValue(1));
//            Assert.AreEqual(4, buffer.GetValue(1));

//            // Verify it was received once
//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 1), Times.Once);
//        }

//        [TestMethod]
//        public void SetValue_Program()
//        {
//            _mixBlock = SwitcherMixBlock.NewProgPrevSameInputs(new SwitcherBusInput(4, ""), new(13, ""));
//            var feature = Create();

//            feature.SetBusValue(0, 13);
//            feature.SetBusValue(0, 4);

//            _mocks.Switcher.Verify(m => m.PostValue(_index, 0, 13), Times.Once);
//            _mocks.Switcher.Verify(m => m.PostValue(_index, 0, 4), Times.Once);
//        }

//        [TestMethod]
//        public void SetValue_Preview()
//        {
//            _mixBlock = SwitcherMixBlock.NewProgPrevSameInputs(new SwitcherBusInput(4, ""), new(13, ""));
//            var feature = Create();

//            feature.SetBusValue(1, 13);
//            feature.SetBusValue(1, 4);

//            _mocks.Switcher.Verify(m => m.PostValue(_index, 1, 13), Times.Once);
//            _mocks.Switcher.Verify(m => m.PostValue(_index, 1, 4), Times.Once);
//        }

//        [TestMethod]
//        public void Cut()
//        {
//            Create().Cut();
//            _mocks.Switcher.Verify(m => m.Cut(7), Times.Once);
//        }

//        [TestMethod]
//        public void Refresh()
//        {
//            var buffer = Create();
//            _mocks.Switcher.Setup(m => m.ReceiveValue(7, 0)).Returns(6);
//            _mocks.Switcher.Setup(m => m.ReceiveValue(7, 1)).Returns(8);
//            buffer.Refresh();

//            Assert.AreEqual(6, buffer.GetValue(0));
//            Assert.AreEqual(8, buffer.GetValue(1));

//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 0), Times.Exactly(2));
//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 1), Times.Exactly(2));
//        }

//        [TestMethod]
//        public void Refresh_Known_Program()
//        {
//            var buffer = Create();
//            buffer.RefreshKnown(0, 13);

//            Assert.AreEqual(13, buffer.GetValue(0));
//            Assert.AreEqual(4, buffer.GetValue(1));

//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 0), Times.Once);
//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 1), Times.Once);
//        }

//        [TestMethod]
//        public void Refresh_Known_Preview()
//        {
//            var buffer = Create();
//            buffer.RefreshKnown(1, 13);

//            Assert.AreEqual(2, buffer.GetValue(0));
//            Assert.AreEqual(13, buffer.GetValue(1));

//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 0), Times.Once);
//            _mocks.Switcher.Verify(m => m.ReceiveValue(7, 1), Times.Once);
//        }
//    }
//}
