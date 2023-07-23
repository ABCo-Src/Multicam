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
        public void Dispose_DisposesSwitcher()
        {
            var switcherMock = new Mock<IDummySwitcher>();
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

            switcherRunningStrip.Dispose();

            switcherMock.Verify(m => m.Dispose());
        }
    }
}
