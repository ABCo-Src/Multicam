using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using Moq;

namespace ABCo.Multicam.Tests.Features.Switchers.Interaction
{
    [TestClass]
    public class PerSpecSwitcherInteractionBufferTests
    {
        public record struct Mocks(
            Mock<ISwitcher> Switcher,
            Mock<IServiceSource> ServSource,
            Mock<ISwitcherInteractionBufferFactory> Factory,
            Mock<IMixBlockInteractionBuffer>[] Buffers,
            Mock<ISwitcherEventHandler> EventHandler);

        SwitcherSpecs _switcherSpecs = new();
        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _switcherSpecs = new(
                SwitcherMixBlock.NewProgPrevSameInputs(new(), new SwitcherBusInput(2, ""), new SwitcherBusInput(4, "")),
                SwitcherMixBlock.NewProgPrevSameInputs(new(), new SwitcherBusInput(6, ""), new SwitcherBusInput(8, ""))
            );

            _mocks.Switcher = new();
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(true);
            _mocks.Switcher.Setup(m => m.RefreshSpecs()).Returns(() => _switcherSpecs);

            _mocks.Buffers = new Mock<IMixBlockInteractionBuffer>[] { new(), new() };
            _mocks.EventHandler = new();

            _mocks.Factory = new();
            _mocks.Factory.Setup(m => m.CreateMixBlock(It.IsAny<SwitcherMixBlock>(), 0, It.IsAny<ISwitcher>())).Returns(_mocks.Buffers[0].Object);
            _mocks.Factory.Setup(m => m.CreateMixBlock(It.IsAny<SwitcherMixBlock>(), 1, It.IsAny<ISwitcher>())).Returns(_mocks.Buffers[1].Object);
        }

        public PerSpecSwitcherInteractionBuffer Create()
        {
            var buffer = new PerSpecSwitcherInteractionBuffer(_mocks.Factory.Object);
            buffer.FinishConstruction(_switcherSpecs, _mocks.Switcher.Object);
            return buffer;
        }

        [TestMethod]
        public void Ctor_Disconnected()
        {
            _switcherSpecs = new();
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);

            var feature = Create();
            Assert.AreEqual(_switcherSpecs, feature.Specs);
            Assert.IsFalse(feature.IsConnected);

            _mocks.Switcher.Verify(m => m.SetEventHandler(feature), Times.Once);
            _mocks.Factory.Verify(m => m.CreateMixBlock(It.IsAny<SwitcherMixBlock>(), It.IsAny<int>(), It.IsAny<ISwitcher>()), Times.Never);
        }

        [TestMethod]
        public void Ctor_Connected()
        {
            var feature = Create();
            Assert.AreEqual(_switcherSpecs, feature.Specs);
            Assert.IsTrue(feature.IsConnected);

            _mocks.Switcher.Verify(m => m.SetEventHandler(feature), Times.Once);
            _mocks.Factory.Verify(m => m.CreateMixBlock(_switcherSpecs.MixBlocks[0], 0, _mocks.Switcher.Object));
            _mocks.Factory.Verify(m => m.CreateMixBlock(_switcherSpecs.MixBlocks[1], 1, _mocks.Switcher.Object));
            _mocks.Buffers[0].Verify(m => m.RefreshValues());
            _mocks.Buffers[1].Verify(m => m.RefreshValues());
        }

        [TestMethod]
        public void SetEventHandler()
        {
			var feature = Create();
            feature.SetEventHandler(_mocks.EventHandler.Object);

            _mocks.Buffers[0].Verify(m => m.SetEventHandler(_mocks.EventHandler.Object));
            _mocks.Buffers[1].Verify(m => m.SetEventHandler(_mocks.EventHandler.Object));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void GetProgram(int mixBlock)
        {
            Create().GetProgram(mixBlock);
            Verify1Success1Fail(mixBlock, (i, t) => i.VerifyGet(m => m.Program, t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void GetPreview(int mixBlock)
        {
            Create().GetPreview(mixBlock);
            Verify1Success1Fail(mixBlock, (i, t) => i.VerifyGet(m => m.Preview, t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SendProgram(int mixBlock)
        {
            Create().SendProgram(mixBlock, 198);
            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.SendProgram(198), t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SendProgram_Disconnected(int mixBlock)
        {
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);
            Create().SendProgram(mixBlock, 198);
            _mocks.Buffers[0].Verify(m => m.SendProgram(198), Times.Never);
            _mocks.Buffers[1].Verify(m => m.SendProgram(198), Times.Never);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SendPreview(int mixBlock)
        {
            Create().SendPreview(mixBlock, 124);
            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.SendPreview(124), t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SendPreview_Disconnected(int mixBlock)
        {
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);
            Create().SendPreview(mixBlock, 198);
            _mocks.Buffers[0].Verify(m => m.SendPreview(198), Times.Never);
            _mocks.Buffers[1].Verify(m => m.SendPreview(198), Times.Never);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void OnProgramChange(int mixBlock)
        {
            var info = new SwitcherProgramChangeInfo(mixBlock, 0, 13, null);
            var feature = Create();
            feature.SetEventHandler(_mocks.EventHandler.Object);
            feature.OnProgramChangeFinish(info);

            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.UpdateProg(13), t));

            for (int i = 0; i < 2; i++)
                _mocks.Buffers[i].Verify(m => m.UpdatePrev(13), Times.Never);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(info));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void OnPreviewChange(int mixBlock)
        {
            var info = new SwitcherPreviewChangeInfo(mixBlock, 13, null);
            var feature = Create();
			feature.SetEventHandler(_mocks.EventHandler.Object);
			feature.OnPreviewChangeFinish(info);

            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.UpdatePrev(13), t));

            for (int i = 0; i < 2; i++)
                _mocks.Buffers[i].Verify(m => m.UpdateProg(13), Times.Never);

            _mocks.EventHandler.Verify(m => m.OnPreviewChangeFinish(info));
        }

        [TestMethod]
        public void OnSpecsChange()
        {
            var feature = Create();
            feature.SetEventHandler(_mocks.EventHandler.Object);
            var info = new SwitcherSpecs();
            feature.OnSpecsChange(info);
            _mocks.EventHandler.Verify(m => m.OnSpecsChange(info));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void Cut(int mixBlock)
        {
            Create().Cut(mixBlock);
            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.Cut(), t));
        }

        [TestMethod]
        public void DisposeSwitcher()
        {
            var feature = Create();
            feature.DisposeSwitcher();
            _mocks.Switcher.Verify(m => m.Dispose(), Times.Once);
        }

        void Verify1Success1Fail(int mixBlock, Action<Mock<IMixBlockInteractionBuffer>, Times> per)
        {
            per(_mocks.Buffers[mixBlock], Times.Once());
            per(_mocks.Buffers[(~mixBlock & 1)], Times.Never());
        }
    }
}
