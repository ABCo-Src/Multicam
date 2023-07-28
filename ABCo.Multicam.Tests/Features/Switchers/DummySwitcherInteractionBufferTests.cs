using ABCo.Multicam.Core.Features.Switchers;
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

        [TestInitialize]
        public void MakeMocks()
        {
            _mocks.DummySwitcherSpecs = new();
            _mocks.DummySwitcher = Mock.Get(Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == _mocks.DummySwitcherSpecs));
        }

        public DummySwitcherInteractionBuffer Create() => new(_mocks.DummySwitcher.Object);

        [TestMethod]
        public void GetValue()
        {
            Create().GetValue(5, 7);
            _mocks.DummySwitcher.Verify(v => v.ReceiveValueAsync(5, 7));
        }

        [TestMethod]
        public async Task SetValue()
        {
            await Create().SetValueAsync(5, 7, 9);
            _mocks.DummySwitcher.Verify(v => v.SendValueAsync(5, 7, 9));
        }

        [TestMethod]
        public void Dispose()
        {
            Create().Dispose();
            _mocks.DummySwitcher.Verify(v => v.Dispose());
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
