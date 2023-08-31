using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
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
    public class DynamicSwitcherInteractionBufferTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServSource,
            Mock<ISwitcherFactory> SwitcherFactory,
            Mock<ISwitcherEventHandler> EventHandler,
            Mock<ISwitcher>[] Switchers,
            Mock<IPerSpecSwitcherInteractionBuffer>[] Buffers
        );

        Mocks _mocks;

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.Buffers = new Mock<IPerSpecSwitcherInteractionBuffer>[] { new(), new(), new() };
            _mocks.Switchers = new Mock<ISwitcher>[] { new(), new(), new() };
            _mocks.EventHandler = new();

            _mocks.SwitcherFactory = new();
            _mocks.SwitcherFactory.SetupSequence(m => m.GetSwitcher(It.IsAny<DummySwitcherConfig>()))
                .Returns(_mocks.Switchers[0].Object)
                .Returns(_mocks.Switchers[1].Object)
                .Returns(_mocks.Switchers[2].Object);

            _mocks.ServSource = new();
            _mocks.ServSource.SetupSequence(m => m.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(It.IsAny<SwitcherSpecs>(), It.IsAny<ISwitcher>()))
                .Returns(_mocks.Buffers[0].Object)
                .Returns(_mocks.Buffers[1].Object)
                .Returns(_mocks.Buffers[2].Object);
        }

        public DynamicSwitcherInteractionBuffer Create()
        {
            var swapper = new DynamicSwitcherInteractionBuffer(_mocks.ServSource.Object, _mocks.SwitcherFactory.Object);
            swapper.FinishConstruction(_mocks.EventHandler.Object);

            for (int i = 0; i < 3; i++)
                _mocks.Switchers[0].Setup(m => m.RefreshSpecs()).Callback(() => swapper.OnSpecsChange(new()));

            return swapper;
        }

        [TestMethod]
        public void Ctor()
        {
            var swapper = Create();

            _mocks.SwitcherFactory.Verify(m => m.GetSwitcher(It.IsAny<DummySwitcherConfig>()));
            _mocks.ServSource.Verify(m => m.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(It.IsAny<SwitcherSpecs>(), _mocks.Switchers[0].Object));
            Assert.AreEqual(_mocks.Buffers[0].Object, swapper.CurrentBuffer);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ChangeSwitcher(bool isConnected)
        {
            _mocks.Switchers[1].Setup(m => m.IsConnected).Returns(isConnected);

            var swapper = Create();
            var config = new DummySwitcherConfig();
            swapper.ChangeSwitcher(config);

            // Verify old switcher is detached
            _mocks.Buffers[0].Verify(m => m.DisposeSwitcher());
			_mocks.Buffers[0].Verify(m => m.SetEventHandler(null));

			// Verify new switcher is attached
			_mocks.SwitcherFactory.Verify(m => m.GetSwitcher(config), Times.Once);
            _mocks.ServSource.Verify(m => m.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(It.IsAny<SwitcherSpecs>(), _mocks.Switchers[1].Object));
            _mocks.Buffers[1].Verify(m => m.SetEventHandler(swapper));
            Assert.AreEqual(_mocks.Buffers[1].Object, swapper.CurrentBuffer);

            // Verify the specs were updated (if connected)
            _mocks.Switchers[1].Verify(m => m.RefreshSpecs(), isConnected ? Times.Once : Times.Never);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ChangeSwitcher_SecondTime(bool isConnected)
        {
            _mocks.Switchers[2].Setup(m => m.IsConnected).Returns(isConnected);

            var swapper = Create();

            var config = new DummySwitcherConfig();
            swapper.ChangeSwitcher(new DummySwitcherConfig());
            swapper.ChangeSwitcher(config);

			// Verify old switcher is detached
			_mocks.Buffers[1].Verify(m => m.SetEventHandler(null));
			_mocks.Buffers[1].Verify(m => m.DisposeSwitcher());

            // Verify new switcher is attached
            _mocks.SwitcherFactory.Verify(m => m.GetSwitcher(config), Times.Once);
            _mocks.ServSource.Verify(m => m.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(It.IsAny<SwitcherSpecs>(), _mocks.Switchers[2].Object));
			_mocks.Buffers[2].Verify(m => m.SetEventHandler(swapper));
			Assert.AreEqual(_mocks.Buffers[2].Object, swapper.CurrentBuffer);

            // Verify the specs were updated (if connected)
            _mocks.Switchers[2].Verify(m => m.RefreshSpecs(), isConnected ? Times.Once : Times.Never);
        }

        //[TestMethod]
        //public async Task ChangeSwitcher_SwitcherNotDisposedDuringBackground()
        //{
        //    var swapper = Create();

        //    _mocks.ServSource.Setup(m => m.GetBackground<ISwitcherInteractionBuffer, ISwitcher>(It.IsAny<ISwitcher>())).Callback(() =>
        //    {
        //        // Assert that we have not yet disposed the old buffer
        //        _mocks.Buffers[0].Verify(m => m.DisposeSwitcher(), Times.Never);
        //    });

        //    await swapper.ChangeSwitcher(new DummySwitcherConfig());
        //}

        [TestMethod]
        public void OnProgramChangeFinish()
        {
            var swapper = Create();
            var info = new SwitcherProgramChangeInfo();
            swapper.OnProgramChangeFinish(info);
            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(info));
        }

        [TestMethod]
        public void OnPreviewChangeFinish()
        {
            var swapper = Create();
            var info = new SwitcherPreviewChangeInfo();
            swapper.OnPreviewChangeFinish(info);
            _mocks.EventHandler.Verify(m => m.OnPreviewChangeFinish(info));
        }

        [TestMethod]
        public void OnSpecsChange()
        {
            var swapper = Create();
            var info = new SwitcherSpecs();
            swapper.OnSpecsChange(info);

            _mocks.ServSource.Verify(m => m.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(info, _mocks.Switchers[0].Object));
            Assert.AreEqual(_mocks.Buffers[1].Object, swapper.CurrentBuffer);

            _mocks.EventHandler.Verify(m => m.OnSpecsChange(info));
        }

        [TestMethod]
        public void OnSpecsChange_PostSwitch()
        {
            var swapper = Create();
            var info = new SwitcherSpecs();
            swapper.ChangeSwitcher(new DummySwitcherConfig());
            swapper.OnSpecsChange(info);

            _mocks.ServSource.Verify(m => m.Get<IPerSpecSwitcherInteractionBuffer, SwitcherSpecs, ISwitcher>(info, _mocks.Switchers[1].Object));
            Assert.AreEqual(_mocks.Buffers[2].Object, swapper.CurrentBuffer);

            _mocks.EventHandler.Verify(m => m.OnSpecsChange(info));
        }

        [TestMethod]
        public void Dispose()
        {
            Create().Dispose();
            _mocks.Buffers[0].Verify(m => m.DisposeSwitcher());
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