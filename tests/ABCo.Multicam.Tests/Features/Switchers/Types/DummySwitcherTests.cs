using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Core;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using Moq;

namespace ABCo.Multicam.Tests.Features.Switchers.Types
{
    [TestClass]
    public class DummySwitcherTests
    {
        SwitcherSpecs? _sentSpecs;
        SwitcherProgramChangeInfo? _changedProgramValue;
        SwitcherPreviewChangeInfo? _changedPreviewValue;

        Mock<ISwitcherEventHandler> _eventHandler = null!;
        VirtualSwitcherConfig _config = null!;

        [TestInitialize]
        public void SetupMocks()
        {
            _sentSpecs = null;
            _changedProgramValue = null;
            _changedPreviewValue = null;
            _eventHandler = new();
            _eventHandler.Setup(m => m.OnSpecsChange(It.IsAny<SwitcherSpecs>())).Callback<SwitcherSpecs>(s => _sentSpecs = s);
            _eventHandler.Setup(m => m.OnProgramValueChange(It.IsAny<SwitcherProgramChangeInfo>())).Callback<SwitcherProgramChangeInfo>(s => _changedProgramValue = s);
            _eventHandler.Setup(m => m.OnPreviewValueChange(It.IsAny<SwitcherPreviewChangeInfo>())).Callback<SwitcherPreviewChangeInfo>(s => _changedPreviewValue = s);
            _config = new VirtualSwitcherConfig(4);
        }

        public VirtualSwitcher Create(ISwitcherEventHandler? eventHandler)
        {
            var obj = new VirtualSwitcher(_config);
            obj.SetEventHandler(eventHandler);
            return obj;
        }

        public VirtualSwitcher Create() => Create(_eventHandler.Object);

        [TestMethod]
        public void RefreshConnectionState()
        {
            Create().RefreshConnectionStatus();
            _eventHandler.Verify(m => m.OnConnectionStateChange(true));
        }

        [TestMethod]
        public void RefreshSpecs_NoEventHandler() => Create(null).RefreshSpecs();

        [TestMethod]
        public void RefreshSpecs_ZeroMixBlocks()
        {
            _config = new VirtualSwitcherConfig();
            Create().RefreshSpecs();

            AssertGeneralSpecsInfo();
            Assert.AreEqual(0, _sentSpecs!.MixBlocks.Count);
        }

        [TestMethod]
        public void RefreshSpecs_ZeroInputMixBlock()
        {
            _config = new VirtualSwitcherConfig(0);
            Create().RefreshSpecs();

            AssertGeneralSpecsInfo();
            Assert.AreEqual(1, _sentSpecs!.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, _sentSpecs.MixBlocks[0].NativeType);
            Assert.AreEqual(0, _sentSpecs.MixBlocks[0].ProgramInputs.Count);
            AssertFeatures(_sentSpecs.MixBlocks[0].SupportedFeatures);
            Assert.AreEqual(_sentSpecs.MixBlocks[0].ProgramInputs, _sentSpecs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void RefreshSpecs_OneMixBlock4In()
        {
            Create().RefreshSpecs();

            AssertGeneralSpecsInfo();
            Assert.AreEqual(1, _sentSpecs!.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, _sentSpecs.MixBlocks[0].NativeType);
            Assert.AreEqual(4, _sentSpecs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(_sentSpecs.MixBlocks[0].ProgramInputs);
            AssertFeatures(_sentSpecs.MixBlocks[0].SupportedFeatures);
            Assert.AreEqual(_sentSpecs.MixBlocks[0].ProgramInputs, _sentSpecs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void RefreshSpecs_TwoMixBlocks()
        {
            _config = new(2, 2);
            Create().RefreshSpecs();

            AssertGeneralSpecsInfo();

            var specs = _sentSpecs!;
            Assert.AreEqual(2, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[1].NativeType);

            // Mix Block 1
            Assert.AreEqual(2, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            AssertFeatures(specs.MixBlocks[0].SupportedFeatures);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);

            // Mix Block 2
            Assert.AreEqual(2, specs.MixBlocks[1].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[1].ProgramInputs);
            AssertFeatures(specs.MixBlocks[1].SupportedFeatures);
            Assert.AreEqual(specs.MixBlocks[1].ProgramInputs, specs.MixBlocks[1].PreviewInputs);
        }

        private void AssertGeneralSpecsInfo()
        {
            Assert.IsNotNull(_sentSpecs);
            Assert.IsFalse(_sentSpecs.CanChangeConnection);
        }

        void AssertInputsList(IReadOnlyList<SwitcherBusInput> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                Assert.AreEqual(i + 1, inputs[i].Id);
                Assert.AreEqual("Cam " + (i + 1), inputs[i].Name);
            }
        }

        void AssertFeatures(SwitcherMixBlockFeatures features)
        {
            Assert.IsTrue(features.SupportsDirectProgramModification);
            Assert.IsTrue(features.SupportsDirectPreviewAccess);
            Assert.IsFalse(features.SupportsCutAction);
            Assert.IsTrue(features.SupportsAutoAction);
            Assert.IsFalse(features.SupportsCutBusModeChanging);
            Assert.IsFalse(features.SupportsCutBusSwitching);
            Assert.IsFalse(features.SupportsCutBusCutMode);
            Assert.IsFalse(features.SupportsCutBusAutoMode);
        }

        [TestMethod]
        public void RefreshProgram_NoEventHandler() => Create(null).RefreshProgram(0);

        [TestMethod]
        public void RefreshProgram_OneMB()
        {
            Create().RefreshProgram(0);
            Assert.IsNotNull(_changedProgramValue);
            Assert.AreEqual(0, _changedProgramValue.Value.MixBlock);
            Assert.AreEqual(1, _changedProgramValue.Value.NewValue);
        }

        [TestMethod]
        public void RefreshPreview_NoEventHandler() => Create(null).RefreshPreview(0);

        [TestMethod]
        public void RefreshPreview_OneMB()
        {
            Create().RefreshPreview(0);
            Assert.IsNotNull(_changedPreviewValue);
            Assert.AreEqual(0, _changedPreviewValue.Value.MixBlock);
            Assert.AreEqual(1, _changedPreviewValue.Value.NewValue);
        }

        [TestMethod]
        public void RefreshAndSendProgram_InvalidMixBlock()
        {
            var dummy = Create();
            Assert.ThrowsException<ArgumentException>(() => dummy.RefreshProgram(1));
            Assert.ThrowsException<ArgumentException>(() => dummy.RefreshProgram(-1));
            Assert.ThrowsException<ArgumentException>(() => dummy.SendProgramValue(1, 3));
            Assert.ThrowsException<ArgumentException>(() => dummy.SendProgramValue(-1, 3));
        }

        [TestMethod]
        public void RefreshAndSendPreview_InvalidMixBlock()
        {
            var dummy = Create();
            Assert.ThrowsException<ArgumentException>(() => dummy.RefreshPreview(1));
            Assert.ThrowsException<ArgumentException>(() => dummy.RefreshPreview(-1));
            Assert.ThrowsException<ArgumentException>(() => dummy.SendPreviewValue(1, 3));
            Assert.ThrowsException<ArgumentException>(() => dummy.SendPreviewValue(-1, 3));
        }

        [TestMethod]
        public void SendProgramValue_NoEventHandler() => Create(null).SendProgramValue(0, 1);

        [TestMethod]
        public void SendProgramValue_InvalidInput()
        {
            _config = new(2);
            var dummy = Create();
            Assert.ThrowsException<ArgumentException>(() => dummy.SendProgramValue(0, -1));
            Assert.ThrowsException<ArgumentException>(() => dummy.SendProgramValue(0, 3));
        }

        [TestMethod]
        public void SendPreviewValue_NoEventHandler() => Create(null).SendPreviewValue(0, 1);

        [TestMethod]
        public void SendPreviewValue_InvalidInput()
        {
            _config = new(2);
            var dummy = Create();
            Assert.ThrowsException<ArgumentException>(() => dummy.SendPreviewValue(0, -1));
            Assert.ThrowsException<ArgumentException>(() => dummy.SendPreviewValue(0, 3));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SendProgramValue(int mixBlock)
        {
            _config = new(4, 4);
            var dummy = Create();
            dummy.SendProgramValue(mixBlock, 2);

            // Assert it sent out the event immediately
            Assert.AreEqual(mixBlock, _changedProgramValue!.Value.MixBlock);
            Assert.AreEqual(2, _changedProgramValue!.Value.NewValue);

            // Make sure it's permanent
            dummy.RefreshProgram(mixBlock);
            Assert.AreEqual(2, _changedProgramValue!.Value.NewValue);

            // Make sure preview was not changed.
            dummy.RefreshPreview(mixBlock);
            Assert.AreEqual(1, _changedPreviewValue!.Value.NewValue); 
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SendPreviewValue(int mixBlock)
        {
            _config = new(4, 4);
            var dummy = Create();
            dummy.SendPreviewValue(mixBlock, 2);

            // Assert it sent out the event immediately
            Assert.AreEqual(mixBlock, _changedPreviewValue!.Value.MixBlock);
            Assert.AreEqual(2, _changedPreviewValue!.Value.NewValue);

            // Make sure it's permanent
            dummy.RefreshPreview(mixBlock);
            Assert.AreEqual(2, _changedPreviewValue!.Value.NewValue);

            // Make sure nothing else was changed.
            dummy.RefreshProgram(mixBlock);
            Assert.AreEqual(1, _changedProgramValue!.Value.NewValue);
        }

        [TestMethod]
        public void Dispose_DoesNotThrow() => Create().Dispose();
    }
}