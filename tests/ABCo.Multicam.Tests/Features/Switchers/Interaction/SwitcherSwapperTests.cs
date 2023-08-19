using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers.Interaction
{
    [TestClass]
    public class SwitcherSwapperTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServSource,
            Mock<ISwitcherFactory> SwitcherFactory,
            Mock<ISwitcher>[] Switchers,
            Mock<ISwitcherInteractionBuffer>[] Buffers
        );

        Mocks _mocks;

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.Buffers = new Mock<ISwitcherInteractionBuffer>[] { new(), new(), new() };
            _mocks.Switchers = new Mock<ISwitcher>[] { new(), new(), new() };

            _mocks.SwitcherFactory = new();
            _mocks.SwitcherFactory.SetupSequence(m => m.GetSwitcher(It.IsAny<DummySwitcherConfig>()))
                .Returns(_mocks.Switchers[0].Object)
                .Returns(_mocks.Switchers[1].Object)
                .Returns(_mocks.Switchers[2].Object);

            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<ISwitcherInteractionBuffer, ISwitcher>(It.IsAny<ISwitcher>())).Returns(_mocks.Buffers[0].Object);
            _mocks.ServSource.SetupSequence(m => m.GetBackground<ISwitcherInteractionBuffer, ISwitcher>(It.IsAny<ISwitcher>()))
                .ReturnsAsync(_mocks.Buffers[1].Object)
                .ReturnsAsync(_mocks.Buffers[2].Object);
        }

        public SwitcherSwapper Create() => new(_mocks.ServSource.Object, _mocks.SwitcherFactory.Object);

        [TestMethod]
        public void Ctor()
        {
            var swapper = Create();

            _mocks.SwitcherFactory.Verify(m => m.GetSwitcher(It.IsAny<DummySwitcherConfig>()));
            _mocks.ServSource.Verify(m => m.Get<ISwitcherInteractionBuffer, ISwitcher>(_mocks.Switchers[0].Object));
            Assert.AreEqual(_mocks.Buffers[0].Object, swapper.CurrentBuffer);
        }

        [TestMethod]
        public async Task ChangeSwitcher()
        {
            var swapper = Create();

            var config = new DummySwitcherConfig();
            await swapper.ChangeSwitcher(config);

            _mocks.SwitcherFactory.Verify(m => m.GetSwitcher(config), Times.Once);
            _mocks.Buffers[0].Verify(m => m.DisposeSwitcher());
            _mocks.ServSource.Verify(m => m.GetBackground<ISwitcherInteractionBuffer, ISwitcher>(_mocks.Switchers[1].Object));
            Assert.AreEqual(_mocks.Buffers[1].Object, swapper.CurrentBuffer);
        }

        [TestMethod]
        public async Task ChangeSwitcher_SecondTime()
        {
            var swapper = Create();

            var config = new DummySwitcherConfig();
            await swapper.ChangeSwitcher(new DummySwitcherConfig());
            await swapper.ChangeSwitcher(config);

            _mocks.SwitcherFactory.Verify(m => m.GetSwitcher(config), Times.Once);
            _mocks.Buffers[1].Verify(m => m.DisposeSwitcher());
            _mocks.ServSource.Verify(m => m.GetBackground<ISwitcherInteractionBuffer, ISwitcher>(_mocks.Switchers[2].Object));
            Assert.AreEqual(_mocks.Buffers[2].Object, swapper.CurrentBuffer);
        }

        [TestMethod]
        public async Task ChangeSwitcher_SwitcherNotDisposedDuringBackground()
        {
            var swapper = Create();

            _mocks.ServSource.Setup(m => m.GetBackground<ISwitcherInteractionBuffer, ISwitcher>(It.IsAny<ISwitcher>())).Callback(() =>
            {
                // Assert that we have not yet disposed the old buffer
                _mocks.Buffers[0].Verify(m => m.DisposeSwitcher(), Times.Never);
            });

            await swapper.ChangeSwitcher(new DummySwitcherConfig());
        }


            //[TestMethod]
            //public async Task ChangeSwitcher_OldNotDisposedDuringAwaits()
            //{
            //    _mocks.Factory.Setup(m => m.CreateAsync(It.IsAny<ISwitcher>())).ReturnsAsync(() =>
            //    {
            //        _mocks.Buffers[0].Verify(m => m.Dispose(), Times.Never);
            //        return _mocks.Buffers[1].Object;
            //    });

            //    var feature = Create();
            //    await feature.ChangeSwitcherConfigAsync(_mocks.NewISwitcher.Object);
            //}
        }
}