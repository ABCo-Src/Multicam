using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
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
    public class MixBlockInteractionBufferTests
    {
        public record struct Mocks(
            Mock<ISwitcher> Switcher,
            Mock<IMixBlockInteractionEmulator> Emulator,
            Mock<ISwitcherInteractionBufferFactory> Factory
        );

        SwitcherMixBlock? _mixBlock = null;
        SwitcherMixBlockFeatures _features = new();
        int _mixBlockIndex;
        Mocks _mocks;

        RetrospectiveFadeInfo? _singleUseCacheChangeVal;

        [TestInitialize]
        public void InitMocks()
        {
            _features = new SwitcherMixBlockFeatures();
            _singleUseCacheChangeVal = null;

            _mixBlockIndex = 13;
            _mocks.Switcher = new();
            _mocks.Emulator = new();

            _mocks.Factory = new();
            _mocks.Factory
                .Setup(m => m.CreateMixBlockEmulator(It.IsAny<SwitcherMixBlock>(), _mixBlockIndex, _mocks.Switcher.Object, It.IsAny<IMixBlockInteractionBuffer>()))
                .Returns(_mocks.Emulator.Object);

            _mocks.Switcher.Setup(m => m.ReceiveValue(_mixBlockIndex, 0)).Returns(2);
            _mocks.Switcher.Setup(m => m.ReceiveValue(_mixBlockIndex, 1)).Returns(4);
            _mocks.Switcher.Setup(m => m.GetCutBusMode(_mixBlockIndex)).Returns(CutBusMode.Auto);

            //_mocks.Emulator.Setup(m => m.TrySetProgWithPreviewThenCutAction()).Returns(false);
            //_mocks.Emulator.Setup(m => m.TrySetProgWithCutBusCutMode()).Returns(false);
        }

        MixBlockInteractionBuffer Create()
        {
            _mixBlock ??= SwitcherMixBlock.NewProgPrevSameInputs(_features, new SwitcherBusInput(3, ""), new(13, ""));

            var feature = new MixBlockInteractionBuffer(_mixBlock, _mixBlockIndex, _mocks.Switcher.Object, _mocks.Factory.Object);
            feature.SetCacheChangeExceptRefreshCall(i => Assert.Fail("Cache change triggered")); // Default cache change handler throws unless specifically looked for
            return feature;
        }

        MixBlockInteractionBuffer CreateWithOneTimeCacheChangeCall()
        {
            var buffer = Create();
            buffer.SetCacheChangeExceptRefreshCall(i =>
            {
                if (_singleUseCacheChangeVal != null) Assert.Fail("Cache change already triggered!");
                _singleUseCacheChangeVal = i;
            });
            return buffer;
        }

        void VerifyCacheChangeCall()
        {
            Assert.AreEqual(new RetrospectiveFadeInfo(), _singleUseCacheChangeVal);
        }

        [TestMethod]
        public void Ctor_Program_Native()
        {
            _features = new SwitcherMixBlockFeatures();

            var buffer = Create();
            Assert.AreEqual(2, buffer.Program);
            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 0), Times.Once);
            _mocks.Factory.Verify(m => m.CreateMixBlockEmulator(_mixBlock!, _mixBlockIndex, _mocks.Switcher.Object, buffer));
        }

        [TestMethod]
        public void Ctor_Preview_Native()
        {
            _features = new SwitcherMixBlockFeatures(supportsDirectPreviewAccess: true);

            var buffer = Create();
            Assert.AreEqual(4, buffer.Preview);
            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 1), Times.Once);
            _mocks.Factory.Verify(m => m.CreateMixBlockEmulator(_mixBlock!, _mixBlockIndex, _mocks.Switcher.Object, buffer));
        }

        [TestMethod]
        public void Ctor_Preview_EmulatedWithInputs()
        {
            var buffer = Create();
            Assert.AreEqual(3, buffer.Preview);
            _mocks.Switcher.Verify(m => m.ReceiveValue(13, 1), Times.Never);
            _mocks.Factory.Verify(m => m.CreateMixBlockEmulator(_mixBlock!, _mixBlockIndex, _mocks.Switcher.Object, buffer));
        }

        [TestMethod]
        public void Ctor_Preview_EmulatedNoInputs()
        {
            _mixBlock = SwitcherMixBlock.NewProgPrevSameInputs(_features);

            var buffer = Create();
            Assert.AreEqual(0, buffer.Preview);
            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 1), Times.Never);
            _mocks.Factory.Verify(m => m.CreateMixBlockEmulator(_mixBlock!, _mixBlockIndex, _mocks.Switcher.Object, buffer));
        }

        [TestMethod]
        public void Ctor_CutBusMode_Native()
        {
            _features = new(supportsCutBusModeChanging: true);

            var buffer = Create();
            Assert.AreEqual(CutBusMode.Auto, buffer.CutBusMode);
            _mocks.Switcher.Verify(m => m.GetCutBusMode(_mixBlockIndex), Times.Once);
            _mocks.Factory.Verify(m => m.CreateMixBlockEmulator(_mixBlock!, _mixBlockIndex, _mocks.Switcher.Object, buffer));
        }

        [TestMethod]
        public void Ctor_CutBusMode_Emulated()
        {
            _mixBlock = SwitcherMixBlock.NewProgPrevSameInputs(_features);

            var buffer = Create();
            Assert.AreEqual(CutBusMode.Cut, buffer.CutBusMode);
            _mocks.Switcher.Verify(m => m.GetCutBusMode(_mixBlockIndex), Times.Never);
            _mocks.Factory.Verify(m => m.CreateMixBlockEmulator(_mixBlock!, _mixBlockIndex, _mocks.Switcher.Object, buffer));
        }

        [TestMethod]
        public void SetProgram_Native()
        {
            _features = new(supportsDirectProgramModification: true);
            var feature = Create();

            feature.SetProgram(13);
            feature.SetProgram(4);

            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 0, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 0, 4), Times.Once);

            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Never);
        }

        [TestMethod]
        public void SetProgram_Emulated1()
        {
            _mocks.Emulator.Setup(m => m.TrySetProgWithPreviewThenCut(24)).Returns(true);

            Create().SetProgram(24);

            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 0, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Never);
        }

        [TestMethod]
        public void SetProgram_Emulated2()
        {
            _mocks.Emulator.Setup(m => m.TrySetProgWithCutBusCut(24)).Returns(true);

            Create().SetProgram(24);

            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 0, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Never);
        }

        [TestMethod]
        public void SetProgram_Emulated3()
        {
            _mocks.Emulator.Setup(m => m.TrySetProgWithCutBusAuto(24)).Returns(true);

            Create().SetProgram(24);

            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 0, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Once);
        }

        [TestMethod]
        public void SetProgram_CannotEmulate()
        {
            var feature = CreateWithOneTimeCacheChangeCall();
            feature.SetProgram(24);

            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 0, 24), Times.Never);
            _mocks.Emulator.Verify(m => m.TrySetProgWithPreviewThenCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusCut(24), Times.Once);
            _mocks.Emulator.Verify(m => m.TrySetProgWithCutBusAuto(24), Times.Once);

            VerifyCacheChangeCall();
            Assert.AreEqual(24, feature.Program);
        }

        [TestMethod]
        public void SetPreview_Native()
        {
            _features = new(supportsDirectPreviewAccess: true);
            var feature = Create();

            feature.SetPreview(13);
            feature.SetPreview(4);

            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 1, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 1, 4), Times.Once);
        }

        [TestMethod]
        public void SetPreview_CannotEmulate()
        {
            var feature = CreateWithOneTimeCacheChangeCall();
            feature.SetPreview(13);

            VerifyCacheChangeCall();
            Assert.AreEqual(13, feature.Preview);
            _mocks.Switcher.Verify(m => m.PostValue(_mixBlockIndex, 1, 13), Times.Never);
        }

        [TestMethod]
        public void Cut_Native()
        {
            _features = new(supportsCutAction: true);
            Create().Cut();
            _mocks.Switcher.Verify(m => m.Cut(_mixBlockIndex), Times.Once);
            _mocks.Emulator.Verify(m => m.CutWithSetProgAndPrev(), Times.Never);
        }

        [TestMethod]
        public void Cut_Emulated()
        {
            Create().Cut();
            _mocks.Switcher.Verify(m => m.Cut(_mixBlockIndex), Times.Never);
            _mocks.Emulator.Verify(m => m.CutWithSetProgAndPrev(), Times.Once);
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
        public void RefreshCache_Program()
        {
            var buffer = Create();

            _mocks.Switcher.Setup(m => m.ReceiveValue(_mixBlockIndex, 0)).Returns(8);
            buffer.RefreshCache();

            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 0), Times.Exactly(2));
            Assert.AreEqual(8, buffer.Program);
        }

        [TestMethod]
        public void RefreshCache_PreviewNative()
        {
            _features = new SwitcherMixBlockFeatures(supportsDirectPreviewAccess: true);
            var buffer = Create();

            _mocks.Switcher.Setup(m => m.ReceiveValue(_mixBlockIndex, 1)).Returns(17);
            buffer.RefreshCache();

            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 1), Times.Exactly(2));
            Assert.AreEqual(17, buffer.Preview);
        }

        [TestMethod]
        public void RefreshCache_PreviewEmulated()
        {
            var buffer = Create();

            _mocks.Switcher.Setup(m => m.ReceiveValue(_mixBlockIndex, 1)).Returns(17);
            buffer.RefreshCache();

            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 1), Times.Never);
            Assert.AreEqual(3, buffer.Preview);
        }

        [TestMethod]
        public void RefreshWithKnownProg()
        {
            var feature = Create();
            feature.RefreshWithKnownProg(27);
            Assert.AreEqual(27, feature.Program);
            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 0), Times.Once);
        }

        [TestMethod]
        public void RefreshWithKnownPrev()
        {
            var feature = Create();
            feature.RefreshWithKnownPrev(76);
            Assert.AreEqual(76, feature.Preview);
            _mocks.Switcher.Verify(m => m.ReceiveValue(_mixBlockIndex, 1), Times.Never);
        }

        //[TestMethod]
        //public void Refresh()
        //{
        //    var buffer = Create();
        //    _mocks.Switcher.Setup(m => m.ReceiveValue(7, 0)).Returns(6);
        //    buffer.Refresh();

        //    Assert.AreEqual(6, buffer.GetValue(0));
        //    Assert.AreEqual(0, buffer.GetValue(1));

        //    _mocks.Switcher.Verify(m => m.ReceiveValue(7, 0), Times.Exactly(2));
        //    _mocks.Switcher.Verify(m => m.ReceiveValue(7, 1), Times.Never);
        //}

        //[TestMethod]
        //public void Refresh_Known_Program()
        //{
        //    var buffer = Create();
        //    buffer.RefreshKnown(0, 13);

        //    Assert.AreEqual(13, buffer.GetValue(0));
        //    Assert.AreEqual(4, buffer.GetValue(1));

        //    _mocks.Switcher.Verify(m => m.ReceiveValue(7, 0), Times.Once);
        //    _mocks.Switcher.Verify(m => m.ReceiveValue(7, 1), Times.Once);
        //}

        //[TestMethod]
        //public void Refresh_Known_Preview()
        //{
        //    var buffer = Create();
        //    buffer.RefreshKnown(1, 13);

        //    Assert.AreEqual(2, buffer.GetValue(0));
        //    Assert.AreEqual(13, buffer.GetValue(1));

        //    _mocks.Switcher.Verify(m => m.ReceiveValue(7, 0), Times.Once);
        //    _mocks.Switcher.Verify(m => m.ReceiveValue(7, 1), Times.Once);
        //}
    }
}
