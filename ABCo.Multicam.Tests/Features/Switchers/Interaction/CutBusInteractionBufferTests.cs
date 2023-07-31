using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers.Interaction
{
    [TestClass]
    public class CutBusInteractionBufferTests
    {
        public record struct Mocks(
            Mock<ISwitcher> Switcher
        );

        SwitcherMixBlock _mixBlock = new();
        int _index;
        Mocks _mocks;

        [TestInitialize]
        public void InitMocks()
        {
            _mixBlock = SwitcherMixBlock.NewCutBus();
            _index = 13;
            _mocks.Switcher = new();
            _mocks.Switcher.Setup(m => m.ReceiveValue(13, 0)).Returns(2);
        }

        CutBusInteractionBuffer Create() => new(_mocks.Switcher.Object, _mixBlock, _index);

        [TestMethod]
        public void GetValue_Program()
        {
            var buffer = Create();

            Assert.AreEqual(2, buffer.GetValue(0));
            Assert.AreEqual(2, buffer.GetValue(0));

            _mocks.Switcher.Verify(m => m.ReceiveValue(13, 0), Times.Once);
        }

        [TestMethod]
        public void Refresh()
        {
            var buffer = Create();
        }

        [TestMethod]
        public void GetValue_Preview_DefaultWithInputs()
        {
            _mixBlock = SwitcherMixBlock.NewCutBus(new(76, ""), new(35, ""));
            var buffer = Create();

            Assert.AreEqual(76, buffer.GetValue(1));
            Assert.AreEqual(76, buffer.GetValue(1));

            _mocks.Switcher.Verify(m => m.ReceiveValue(13, 1), Times.Never);
        }

        [TestMethod]
        public void GetValue_Preview_DefaultNoInputs()
        {
            var buffer = Create();

            Assert.AreEqual(0, buffer.GetValue(1));
            Assert.AreEqual(0, buffer.GetValue(1));

            _mocks.Switcher.Verify(m => m.ReceiveValue(13, 1), Times.Never);
        }

        [TestMethod]
        public void SetValue_Program()
        {
            // TODO: Not actually what we should do for a cut bus!
            _mixBlock = SwitcherMixBlock.NewCutBus(new SwitcherBusInput(4, ""), new(13, ""));
            var feature = Create();

            feature.SetBusValue(0, 13);
            feature.SetBusValue(0, 4);

            _mocks.Switcher.Verify(m => m.PostValue(_index, 0, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.PostValue(_index, 0, 4), Times.Once);
        }

        [TestMethod]
        public void SetValue_Preview()
        {
            _mixBlock = SwitcherMixBlock.NewCutBus(new SwitcherBusInput(4, ""), new(13, ""));
            var feature = Create();

            feature.SetBusValue(1, 13);
            Assert.AreEqual(13, feature.GetValue(1));
            feature.SetBusValue(1, 4);
            Assert.AreEqual(4, feature.GetValue(1));

            _mocks.Switcher.Verify(m => m.PostValue(_index, 1, 13), Times.Never);
            _mocks.Switcher.Verify(m => m.PostValue(_index, 1, 4), Times.Never);
        }
    }
}
