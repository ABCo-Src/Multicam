using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Strips.Switchers
{
    [TestClass]
    public class SwitcherRunningStripTests
    {
        public SwitcherRunningStrip CreateDefault() => new SwitcherRunningStrip(Mock.Of<IDummySwitcher>());
        public SwitcherRunningStrip CreateWithCustomSwitcher(IDummySwitcher switcher) => new SwitcherRunningStrip(switcher);

        [TestMethod]
        public void SwitcherSpecs_Matches()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == mockSpecs);
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);
        }

        [TestMethod]
        public void SwitcherSpecs_NoChangeBetweenGets()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(mockSpecs);

            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);

            // Verify it was only received once
            switcherMock.Verify(m => m.ReceiveSpecs(), Times.Once);

        }

        [TestMethod]
        public void Dispose_DisposesSwitcher()
        {
            var switcherMock = new Mock<IDummySwitcher>();
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

            switcherRunningStrip.Dispose();

            switcherMock.Verify(m => m.Dispose());
        }
    }
}
