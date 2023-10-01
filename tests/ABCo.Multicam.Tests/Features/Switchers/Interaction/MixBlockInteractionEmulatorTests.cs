using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Interaction;
using ABCo.Multicam.Tests.Helpers;
using Moq;

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

            _mocks.Parent.SetupGet(m => m.Program).Returns(5);
            _mocks.Parent.SetupGet(m => m.Preview).Returns(8);
        }

        MixBlockInteractionEmulator Create()
        {
            _mixBlock ??= SwitcherMixBlock.NewProgPrevSameInputs(_features, new SwitcherBusInput(3, ""), new(13, ""));
            return new(_mixBlock, _mixBlockIndex, _mocks.Switcher.Object, _mocks.Parent.Object); 
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        public void TrySetProg_PreviewThenCut_NotPossible(bool canAccessPreview, bool canCut)
        {
            _features = new(supportsDirectPreviewAccess: canAccessPreview, supportsCutAction: canCut);
            Assert.IsFalse(Create().TrySetProgWithPreviewThenCut(13));
            _mocks.Switcher.Verify(m => m.SendPreviewValue(_mixBlockIndex, 13), Times.Never);
            _mocks.Switcher.Verify(m => m.Cut(_mixBlockIndex), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_PreviewThenCut_Possible()
        {
            _features = new(supportsDirectPreviewAccess: true, supportsCutAction: true);

            var sequence = _mocks.Switcher.SetupSequenceTracker(
                m => m.SendPreviewValue(_mixBlockIndex, 13),
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
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Cut), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusCut_NoSwitch_Possible()
        {
            _features = new(supportsCutBusSwitching: true, supportsCutBusCutMode: true);

            Assert.IsTrue(Create().TrySetProgWithCutBusCut(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Cut), Times.Never);
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
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Cut), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusCut_Switch_Possible()
        {
            _mocks.Parent.SetupGet(m => m.CutBusMode).Returns(CutBusMode.Auto);
            _features = new(supportsCutBusSwitching: true, supportsCutBusCutMode: true, supportsCutBusModeChanging: true);

            var sequence = _mocks.Switcher.SetupSequenceTracker(
                m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Cut),
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
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Auto), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusAuto_NoSwitch_Possible()
        {
            _mocks.Parent.SetupGet(m => m.CutBusMode).Returns(CutBusMode.Auto);
            _features = new(supportsCutBusSwitching: true, supportsCutBusAutoMode: true);

            Assert.IsTrue(Create().TrySetProgWithCutBusAuto(13));
            _mocks.Switcher.Verify(m => m.SetCutBus(_mixBlockIndex, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Auto), Times.Never);
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
            _mocks.Switcher.Verify(m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Auto), Times.Never);
        }

        [TestMethod]
        public void TrySetProg_CutBusAuto_Switch_Possible()
        {
            _features = new(supportsCutBusSwitching: true, supportsCutBusAutoMode: true, supportsCutBusModeChanging: true);

            var sequence = _mocks.Switcher.SetupSequenceTracker(
                m => m.SetCutBusMode(_mixBlockIndex, CutBusMode.Auto),
                m => m.SetCutBus(_mixBlockIndex, 13)
            );

            Assert.IsTrue(Create().TrySetProgWithCutBusAuto(13));
            sequence.Verify();
        }

        [TestMethod]
        public void Cut_PrevThenProg()
        {
            var sequence = _mocks.Parent.SetupSequenceTracker(
                m => m.SendPreview(5),
                m => m.SendProgram(8)
            );

            Create().CutWithSetProgAndPrev();

            _mocks.Parent.VerifyGet(m => m.Program);
            _mocks.Parent.VerifyGet(m => m.Preview);

            sequence.Verify();
        }

        [TestMethod]
        public void SetCutBusWithProgSet()
        {
            Create().SetCutBusWithProgSet(27);
            _mocks.Parent.Verify(m => m.SendProgram(27));
        }

        [TestMethod]
        public void TrySetCutBus_PrevThenAuto_Possible()
        {
            _features = new(supportsDirectPreviewAccess: true, supportsAutoAction: true);

            var sequence = _mocks.Parent.SetupSequenceTracker(
                m => m.SendPreview(34),
                m => m.Auto()
            );

            Assert.IsTrue(Create().TrySetCutBusWithPrevThenAuto(34));
            sequence.Verify();
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        public void TrySetCutBus_PrevThenAuto_NotPossible(bool directPreviewAccess, bool autoAction)
        {
            _features = new(supportsDirectPreviewAccess: directPreviewAccess, supportsAutoAction: autoAction);
            Assert.IsFalse(Create().TrySetCutBusWithPrevThenAuto(34));
            _mocks.Parent.Verify(m => m.SendPreview(34), Times.Never);
            _mocks.Parent.Verify(m => m.Auto(), Times.Never);
        }
    }
}
