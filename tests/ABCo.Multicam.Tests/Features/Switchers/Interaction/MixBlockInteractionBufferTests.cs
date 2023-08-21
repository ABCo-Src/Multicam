using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using Moq;
using System;

namespace ABCo.Multicam.Tests.Features.Switchers.Interaction
{
    [TestClass]
    public class MixBlockInteractionBufferTests
    {
        public record struct Mocks(
            Mock<ISwitcher> Switcher,
            Mock<IMixBlockInteractionEmulator> Emulator,
            Mock<ISwitcherEventHandler> EventHandler,
            Mock<ISwitcherInteractionBufferFactory> Factory
        );

        SwitcherMixBlock? _mixBlock = null;
        SwitcherMixBlockFeatures _features = new();
        int _mixBlockIndex;
        Mocks _mocks;

        SwitcherProgramChangeInfo? _programChangeVal;
        SwitcherPreviewChangeInfo? _previewChangeVal;

        [TestInitialize]
        public void InitMocks()
        {
            _features = new SwitcherMixBlockFeatures();
            _programChangeVal = null;

            _mixBlock = null;
            _mixBlockIndex = 13;
            _mocks.Switcher = new();
            _mocks.Emulator = new();

            _mocks.Factory = new();
            _mocks.Factory
                .Setup(m => m.CreateMixBlockEmulator(It.IsAny<SwitcherMixBlock>(), _mixBlockIndex, _mocks.Switcher.Object, It.IsAny<IMixBlockInteractionBuffer>()))
                .Returns(_mocks.Emulator.Object);

            _mocks.EventHandler = new();
            _mocks.EventHandler.Setup(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>())).Callback<SwitcherProgramChangeInfo>(v => _programChangeVal = v);
            _mocks.EventHandler.Setup(m => m.OnPreviewChangeFinish(It.IsAny<SwitcherPreviewChangeInfo>())).Callback<SwitcherPreviewChangeInfo>(v => _previewChangeVal = v);

            _mocks.Switcher.Setup(m => m.GetCutBusMode(_mixBlockIndex)).Returns(CutBusMode.Auto);

            //_mocks.Emulator.Setup(m => m.TrySetProgWithPreviewThenCutAction()).Returns(false);
            //_mocks.Emulator.Setup(m => m.TrySetProgWithCutBusCutMode()).Returns(false);
        }

        MixBlockInteractionBuffer Create()
        {
            _mixBlock ??= SwitcherMixBlock.NewProgPrevSameInputs(_features, new SwitcherBusInput(3, ""), new(13, ""));
            var buffer = new MixBlockInteractionBuffer(_mixBlock, _mixBlockIndex, _mocks.Switcher.Object, _mocks.EventHandler.Object, _mocks.Factory.Object);
            buffer.UpdateProg(2);
            if (_features.SupportsDirectPreviewAccess) buffer.UpdatePrev(4);
            return buffer;
        }

        [TestMethod]
        public void Ctor()
        {
            var buffer = Create();
            _mocks.Factory.Verify(m => m.CreateMixBlockEmulator(_mixBlock!, _mixBlockIndex, _mocks.Switcher.Object, buffer));
            _mocks.Switcher.Verify(m => m.RefreshProgram(_mixBlockIndex), Times.Never);
            _mocks.Switcher.Verify(m => m.RefreshPreview(_mixBlockIndex), Times.Never);
        }

        [TestMethod]
        public void RefreshValues_Program_Native()
        {
            _features = new SwitcherMixBlockFeatures();

            var buffer = Create();
            buffer.RefreshValues();
            Assert.AreEqual(2, buffer.Program);
            _mocks.Switcher.Verify(m => m.RefreshProgram(_mixBlockIndex), Times.Once);
        }

        [TestMethod]
        public void RefreshValues_Preview_Native()
        {
            _features = new SwitcherMixBlockFeatures(supportsDirectPreviewAccess: true);

            var buffer = Create();
            buffer.RefreshValues();
            Assert.AreEqual(4, buffer.Preview);
            _mocks.Switcher.Verify(m => m.RefreshPreview(_mixBlockIndex), Times.Once);
        }

        [TestMethod]
        public void RefreshValues_Preview_EmulatedWithInputs()
        {
            var buffer = Create();
            buffer.RefreshValues();
            Assert.AreEqual(3, buffer.Preview);
            _mocks.Switcher.Verify(m => m.RefreshPreview(_mixBlockIndex), Times.Never);
        }

        [TestMethod]
        public void RefreshValues_Preview_EmulatedNoInputs()
        {
            _mixBlock = SwitcherMixBlock.NewProgPrevSameInputs(_features);

            var buffer = Create();
            buffer.RefreshValues();
            Assert.AreEqual(0, buffer.Preview);
            _mocks.Switcher.Verify(m => m.RefreshPreview(_mixBlockIndex), Times.Never);
        }

        [TestMethod]
        public void RefreshValues_CutBusMode_Native()
        {
            _features = new(supportsCutBusModeChanging: true);

            var buffer = Create();
            buffer.RefreshValues();
            Assert.AreEqual(CutBusMode.Auto, buffer.CutBusMode);
            _mocks.Switcher.Verify(m => m.GetCutBusMode(_mixBlockIndex), Times.Once);
        }

        [TestMethod]
        public void RefreshValues_CutBusMode_Emulated()
        {
            _mixBlock = SwitcherMixBlock.NewProgPrevSameInputs(_features);

            var buffer = Create();
            buffer.RefreshValues();
            Assert.AreEqual(CutBusMode.Cut, buffer.CutBusMode);
            _mocks.Switcher.Verify(m => m.GetCutBusMode(_mixBlockIndex), Times.Never);
        }

        [TestMethod]
        public void SendProgram_Native()
        {
            _features = new(supportsDirectProgramModification: true);
            var feature = Create();

            feature.SendProgram(13);
            feature.SendProgram(4);

            _mocks.Switcher.Verify(m => m.SendProgramValue(_mixBlockIndex, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.SendProgramValue(_mixBlockIndex, 4), Times.Once);

            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Never);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>()), Times.Never);
        }

        [TestMethod]
        public void SendProgram_Emulated1()
        {
            _mocks.Emulator.Setup(m => m.TrySetProgWithPreviewThenCut(24)).Returns(true);

            Create().SendProgram(24);

            _mocks.Switcher.Verify(m => m.SendProgramValue(_mixBlockIndex, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Never);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>()), Times.Never);
        }

        [TestMethod]
        public void SendProgram_Emulated2()
        {
            _mocks.Emulator.Setup(m => m.TrySetProgWithCutBusCut(24)).Returns(true);

            Create().SendProgram(24);

            _mocks.Switcher.Verify(m => m.SendProgramValue(_mixBlockIndex, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Never);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>()), Times.Never);
        }

        [TestMethod]
        public void SendProgram_Emulated3()
        {
            _mocks.Emulator.Setup(m => m.TrySetProgWithCutBusAuto(24)).Returns(true);

            Create().SendProgram(24);

            _mocks.Switcher.Verify(m => m.SendProgramValue(_mixBlockIndex, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Once);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>()), Times.Never);
        }

        [TestMethod]
        public void SendProgram_CannotEmulate()
        {
            var feature = Create();
            feature.SendProgram(24);

            _mocks.Switcher.Verify(m => m.SendProgramValue(_mixBlockIndex, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Once);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>()), Times.Once);
            Assert.AreEqual(13, _programChangeVal!.Value.MixBlock);
            Assert.AreEqual(24, _programChangeVal!.Value.NewValue);

            Assert.AreEqual(24, feature.Program);
        }

        [TestMethod]
        public void SendPreview_Native()
        {
            _features = new(supportsDirectPreviewAccess: true);
            var feature = Create();

            feature.SendPreview(13);
            feature.SendPreview(4);

            _mocks.Switcher.Verify(m => m.SendPreviewValue(_mixBlockIndex, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.SendPreviewValue(_mixBlockIndex, 4), Times.Once);

            _mocks.EventHandler.Verify(m => m.OnPreviewChangeFinish(It.IsAny<SwitcherPreviewChangeInfo>()), Times.Never);
        }

        [TestMethod]
        public void SendPreview_CannotEmulate()
        {
            var feature = Create();
            feature.SendPreview(24);

            Assert.AreEqual(24, feature.Preview);
            _mocks.Switcher.Verify(m => m.SendPreviewValue(_mixBlockIndex, 24), Times.Never);

            _mocks.EventHandler.Verify(m => m.OnPreviewChangeFinish(It.IsAny<SwitcherPreviewChangeInfo>()), Times.Once);
            Assert.AreEqual(13, _previewChangeVal!.Value.MixBlock);
            Assert.AreEqual(24, _previewChangeVal!.Value.NewValue);
        }

        [TestMethod]
        public void Cut_Native()
        {
            _features = new(supportsCutAction: true);
            Create().Cut();
            _mocks.Switcher.Verify(m => m.Cut(_mixBlockIndex), Times.Once);
            _mocks.Emulator.Verify(m => m.CutWithSetProgAndPrev(), Times.Never);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>()), Times.Never);
            _mocks.EventHandler.Verify(m => m.OnPreviewChangeFinish(It.IsAny<SwitcherPreviewChangeInfo>()), Times.Never);
        }

        [TestMethod]
        public void Cut_Emulated()
        {
            Create().Cut();
            _mocks.Switcher.Verify(m => m.Cut(_mixBlockIndex), Times.Never);
            _mocks.Emulator.Verify(m => m.CutWithSetProgAndPrev(), Times.Once);

            _mocks.EventHandler.Verify(m => m.OnProgramChangeFinish(It.IsAny<SwitcherProgramChangeInfo>()), Times.Never);
            _mocks.EventHandler.Verify(m => m.OnPreviewChangeFinish(It.IsAny<SwitcherPreviewChangeInfo>()), Times.Never);
        }

        [TestMethod]
        [DataRow(CutBusMode.Cut)]
        [DataRow(CutBusMode.Auto)]
        public void SetCutBus_Native(CutBusMode mode)
        {
            _features = new(supportsCutBusSwitching: true);
            var buffer = Create();
            buffer.SetCutBusMode(mode);
            buffer.SetCutBus(13);
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetCutBusWithPrevThenAuto(13), Times.Never);
            _mocks.Emulator.Verify(m => m.SetCutBusWithProgSet(13), Times.Never);
        }

        [TestMethod]
        public void SetCutBus_Cut_Emulated()
        {
            Create().SetCutBus(13);
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetCutBusWithPrevThenAuto(13), Times.Never);
            _mocks.Emulator.Verify(m => m.SetCutBusWithProgSet(13), Times.Once);
        }

        [TestMethod]
        public void SetCutBus_Auto_Emulated1()
        {
            _mocks.Emulator.Setup(m => m.TrySetCutBusWithPrevThenAuto(13)).Returns(true);
            var buffer = Create();
            buffer.SetCutBusMode(CutBusMode.Auto);
            buffer.SetCutBus(13);
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetCutBusWithPrevThenAuto(13), Times.Once);
            _mocks.Emulator.Verify(m => m.SetCutBusWithProgSet(13), Times.Never);
        }

        [TestMethod]
        public void SetCutBus_Auto_Emulated2()
        {
            var buffer = Create();
            buffer.SetCutBusMode(CutBusMode.Auto);
            buffer.SetCutBus(13);
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetCutBusWithPrevThenAuto(13), Times.Once);
            _mocks.Emulator.Verify(m => m.SetCutBusWithProgSet(13), Times.Once);
        }

        [TestMethod]
        [DataRow(CutBusMode.Cut)]
        [DataRow(CutBusMode.Auto)]
        public void SetCutBusMode_Native(CutBusMode mode)
        {
            _features = new(supportsCutBusModeChanging: true);
            Create().SetCutBusMode(mode);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, mode), Times.Once);
        }

        [TestMethod]
        [DataRow(CutBusMode.Cut)]
        [DataRow(CutBusMode.Auto)]
        public void SetCutBusMode_Emulated(CutBusMode mode)
        {
            var buffer = Create();
            buffer.SetCutBusMode(mode);
            Assert.AreEqual(mode, buffer.CutBusMode);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, mode), Times.Never);
        }

        [TestMethod]
        public void UpdateProg()
        {
            var feature = Create();
            feature.UpdateProg(27);
            Assert.AreEqual(27, feature.Program);
            _mocks.Switcher.Verify(m => m.RefreshProgram(_mixBlockIndex), Times.Never);
        }

        [TestMethod]
        public void UpdatePrev()
        {
            var feature = Create();
            feature.UpdatePrev(76);
            Assert.AreEqual(76, feature.Preview);
            _mocks.Switcher.Verify(m => m.RefreshPreview(_mixBlockIndex), Times.Never);
        }
    }
}
