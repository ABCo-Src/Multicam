using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using Moq;

namespace ABCo.Multicam.Tests.Features.Switchers
{
	[TestClass]
    public class SwitcherFactoryTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServSource,
            Mock<IDummySwitcher> DummySwitcher
        );

        Mocks _mocks;

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.DummySwitcher = new();
            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<IDummySwitcher, DummySwitcherConfig>(It.IsAny<DummySwitcherConfig>()))
                .Returns(_mocks.DummySwitcher.Object);
        }

        SwitcherFactory Create() => new(_mocks.ServSource.Object);

        [TestMethod]
        public void GetSwitcher_Dummy()
        {
            var config = new DummySwitcherConfig();
            Assert.AreEqual(_mocks.DummySwitcher.Object, Create().GetSwitcher(config));
            _mocks.ServSource.Verify(m => m.Get<IDummySwitcher, DummySwitcherConfig>(config));
        }
    }
}
