using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers.Interaction
{
    [TestClass]
    public class MixBlockInteractionEmulatorTests
    {
        public record struct Mocks(
            Mock<ISwitcher> Switcher,
            Mock<IMixBlockInteractionBuffer> Parent
            );

        SwitcherMixBlock? _mixBlock = null;
        SwitcherMixBlockFeatures _features = new();
        int _mixBlockIndex;
        Mocks _mocks;

        [TestInitialize]
        public void InitMocks()
        {
            _features = new SwitcherMixBlockFeatures();
            _mixBlockIndex = 25;
            _mocks.Switcher = new();

            _mocks.Parent = new();
            _mocks.Parent.SetupGet(m => m.CutBusMode).Returns(CutBusMode.Cut);

            _mocks.Switcher.Setup(m => m.ReceiveValue(_mixBlockIndex, 0)).Returns(2);
            _mocks.Switcher.Setup(m => m.ReceiveValue(_mixBlockIndex, 1)).Returns(4);
        }

        MixBlockInteractionEmulator Create()
        {
            _mixBlock ??= SwitcherMixBlock.NewProgPrevSameInputs(_features, new SwitcherBusInput(3, ""), new(13, ""));
            return new(_mixBlock, _mixBlockIndex, _mocks.Parent.Object, _mocks.Switcher.Object); 
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        public void TrySetProg_PreviewThenCut_NotPossible(bool canAccessPreview, bool canCut)
        {
            _features = new(supportsDirectPreviewAccess: canAccessPreview, supportsCutAction: canCut);
            Assert.IsFalse(Create().TrySetProgWithPreviewThenCut(13));
        }

        [TestMethod]
        public void TrySetProg_PreviewThenCut_Possible()
        {
            _features = new(supportsDirectPreviewAccess: true, supportsCutAction: true);

            var sequence = _mocks.Switcher.SetupSequenceTracker(
                m => m.PostValue(_mixBlockIndex, 0, 13),
                m => m.Cut(_mixBlockIndex)
            );

            Assert.IsTrue(Create().TrySetProgWithPreviewThenCut(13));
            sequence.Verify();
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        public void TrySetProg_CutBusCut_NoSwitch_NotPossible(bool canSetCutBus, bool canUseCutMode)
        {
            _features = new(supportsCutBusSwitching: canSetCutBus, supportsCutBusCutMode: canUseCutMode);
            Assert.IsFalse(Create().TrySetProgWithCutBusCut(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Never);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(CutBusMode.Cut), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusCut_NoSwitch_Possible()
        {
            _features = new(supportsCutBusSwitching: true, supportsCutBusCutMode: true);

            Assert.IsTrue(Create().TrySetProgWithCutBusCut(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(CutBusMode.Cut), Times.Never);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void TrySetProg_CutBusCut_Switch_NotPossible(bool canSetCutBus, bool canUseCutMode)
        {
            _mocks.Parent.SetupGet(m => m.CutBusMode).Returns(CutBusMode.Auto);
            _features = new(supportsCutBusSwitching: canSetCutBus, supportsCutBusCutMode: canUseCutMode, supportsCutBusModeChanging: false);

            Assert.IsFalse(Create().TrySetProgWithCutBusCut(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Never);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(CutBusMode.Cut), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusCut_Switch_Possible()
        {
            _mocks.Parent.SetupGet(m => m.CutBusMode).Returns(CutBusMode.Auto);
            _features = new(supportsCutBusSwitching: true, supportsCutBusCutMode: true, supportsCutBusModeChanging: true);

            var sequence = _mocks.Switcher.SetupSequenceTracker(
                m => m.SetCutBusMode(CutBusMode.Cut),
                m => m.SetCutBus(_mixBlockIndex, 13)
            );

            Assert.IsTrue(Create().TrySetProgWithCutBusCut(13));
            sequence.Verify();
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        public void TrySetProg_CutBusAuto_NoSwitch_NotPossible(bool canSetCutBus, bool canUseAutoMode)
        {
            _mocks.Parent.SetupGet(m => m.CutBusMode).Returns(CutBusMode.Auto);
            _features = new(supportsCutBusSwitching: canSetCutBus, supportsCutBusAutoMode: canUseAutoMode);
            Assert.IsFalse(Create().TrySetProgWithCutBusAuto(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Never);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(CutBusMode.Auto), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusAuto_NoSwitch_Possible()
        {
            _mocks.Parent.SetupGet(m => m.CutBusMode).Returns(CutBusMode.Auto);
            _features = new(supportsCutBusSwitching: true, supportsCutBusAutoMode: true);

            Assert.IsTrue(Create().TrySetProgWithCutBusAuto(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(CutBusMode.Auto), Times.Never);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void TrySetProg_CutBusAuto_Switch_NotPossible(bool canSetCutBus, bool canUseAutoMode)
        {
            _features = new(supportsCutBusSwitching: canSetCutBus, supportsCutBusAutoMode: canUseAutoMode, supportsCutBusModeChanging: false);

            Assert.IsFalse(Create().TrySetProgWithCutBusAuto(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Never);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(CutBusMode.Auto), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusAuto_Switch_Possible()
        {
            _features = new(supportsCutBusSwitching: true, supportsCutBusAutoMode: true, supportsCutBusModeChanging: true);

            var sequence = _mocks.Switcher.SetupSequenceTracker(
                m => m.SetCutBusMode(CutBusMode.Auto),
                m => m.SetCutBus(_mixBlockIndex, 13)
            );

            Assert.IsTrue(Create().TrySetProgWithCutBusAuto(13));
            sequence.Verify();
        }
    }
}
