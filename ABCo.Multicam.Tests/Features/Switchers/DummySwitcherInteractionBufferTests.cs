using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers
{
    [TestClass]
    public class DummySwitcherInteractionBufferTests
    {
        public record struct Mocks(Mock<IDummySwitcher> DummySwitcher, SwitcherSpecs DummySwitcherSpecs);
        Mocks _mocks = new();

        Action<SwitcherBusChangeInfo> _onBusChangeCallback = i => { };

        [TestInitialize]
        public void MakeMocks()
        {
            _mocks.DummySwitcherSpecs = new();
            _mocks.DummySwitcher = new Mock<IDummySwitcher>();
            _mocks.DummySwitcher.Setup(m => m.ReceiveSpecs()).Returns(_mocks.DummySwitcherSpecs);
            _mocks.DummySwitcher.Setup(m => m.SetOnBusChangeFinishCall(It.IsAny<Action<SwitcherBusChangeInfo>>())).Callback<Action<SwitcherBusChangeInfo>>(v => _onBusChangeCallback = v);
        }

        public DummySwitcherInteractionBuffer Create() => new(_mocks.DummySwitcher.Object);

        [TestMethod]
        public void GetValue()
        {
            Create().GetValue(5, 7);
            _mocks.DummySwitcher.Verify(v => v.ReceiveValue(5, 7));
        }

        [TestMethod]
        public void SetValue()
        {
            Create().PostValue(5, 7, 9);
            _mocks.DummySwitcher.Verify(v => v.PostValue(5, 7, 9));
        }

        [TestMethod]
        public void Dispose()
        {
            Create().Dispose();
            _mocks.DummySwitcher.Verify(v => v.Dispose());
        }

        [TestMethod]
        [DataRow(false, 0, 0)]
        [DataRow(false, 1, 1)]
        [DataRow(true, 0, 0)]
        [DataRow(true, 1, 1)]
        public void OnBusChange(bool isKnown, int mixBlock, int bus)
        {
            var buffer = Create();

            bool ran = false;
            buffer.SetOnBusChangeFinishCall(i =>
            {
                Assert.AreEqual(new RetrospectiveFadeInfo(), i);
                ran = true;
            });

            _onBusChangeCallback(new SwitcherBusChangeInfo(isKnown, mixBlock, bus, 0, new RetrospectiveFadeInfo()));

            Assert.IsTrue(ran);
        }

        [TestMethod]
        public void IsConnected()
        {
            Assert.IsTrue(Create().IsConnected);
        }

        [TestMethod]
        public void Specs()
        {
            Assert.AreEqual(_mocks.DummySwitcherSpecs, Create().Specs);
            _mocks.DummySwitcher.Verify(v => v.ReceiveSpecs());
        }

    }
}
