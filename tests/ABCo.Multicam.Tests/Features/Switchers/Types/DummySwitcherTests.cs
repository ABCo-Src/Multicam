using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;

namespace ABCo.Multicam.Tests.Features.Switchers.Types
{
    [TestClass]
    public class DummySwitcherTests
    {
        DummySwitcherConfig _config = null!;

        [TestInitialize]
        public void SetupMocks()
        {
            _config = new DummySwitcherConfig(4);
        }

        public DummySwitcher Create()
        {
            var obj = new DummySwitcher();
            obj.FinishConstruction(_config);
            return obj;
        }

        [TestMethod]
        public void ReceiveSpecs_ZeroMixBlocks()
        {
            _config = new DummySwitcherConfig();
            var dummy = Create();
            Assert.AreEqual(0, dummy.ReceiveSpecs().MixBlocks.Count);
        }

        [TestMethod]
        public void ReceiveSpecs_ZeroInputMixBlock()
        {
            _config = new DummySwitcherConfig(0);
            var dummy = Create();

            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(0, specs.MixBlocks[0].ProgramInputs.Count);
            AssertFeatures(specs.MixBlocks[0].SupportedFeatures);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void ReceiveSpecs_OneMixBlock4In()
        {
            var dummy = Create();
            var specs = dummy.ReceiveSpecs();

            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(4, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            AssertFeatures(specs.MixBlocks[0].SupportedFeatures);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void ReceiveSpecs_TwoMixBlocks()
        {
            _config = new(2, 2);
            var dummy = Create();

            var specs = dummy.ReceiveSpecs();

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
        public void ReceiveValue_Program_OneMB() => Assert.AreEqual(1, Create().ReceiveValue(0, 0));

        [TestMethod]
        public void ReceiveValue_Preview_OneMB() => Assert.AreEqual(1, Create().ReceiveValue(0, 1));

        [TestMethod]
        public void ReceiveAndSendValue_InvalidMixBlock()
        {
            var dummy = Create();
            Assert.ThrowsException<ArgumentException>(() => dummy.ReceiveValue(1, 0));
            Assert.ThrowsException<ArgumentException>(() => dummy.ReceiveValue(-1, 0));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(1, 0, 3));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(-1, 0, 3));
        }

        [TestMethod]
        public void ReceiveAndSendValue_InvalidBus_PreviewProgram()
        {
            var dummy = Create();
            Assert.ThrowsException<ArgumentException>(() => dummy.ReceiveValue(0, -1));
            Assert.ThrowsException<ArgumentException>(() => dummy.ReceiveValue(0, 2));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, -1, 3));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, 2, 3));
        }

        [TestMethod]
        public void SendValue_InvalidInput()
        {
            _config = new(2);
            var dummy = Create();
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, 0, -1));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, 0, 3));
        }

        [TestMethod]
        public void SendValue_Program()
        {
            var dummy = Create();

            dummy.PostValue(0, 0, 2);
            Assert.AreEqual(2, dummy.ReceiveValue(0, 0)); // Impacts program
            Assert.AreEqual(1, dummy.ReceiveValue(0, 1)); // Does not impact preview

            dummy.PostValue(0, 0, 3);
            Assert.AreEqual(3, dummy.ReceiveValue(0, 0)); // Impacts program
            Assert.AreEqual(1, dummy.ReceiveValue(0, 1)); // Does not impact preview
        }

        [TestMethod]
        public void SendValue_FirstMB_Preview()
        {
            var dummy = Create();

            dummy.PostValue(0, 1, 2);
            Assert.AreEqual(2, dummy.ReceiveValue(0, 1)); // Impacts preview
            Assert.AreEqual(1, dummy.ReceiveValue(0, 0)); // Does not impact program

            dummy.PostValue(0, 1, 3);
            Assert.AreEqual(3, dummy.ReceiveValue(0, 1)); // Impacts preview
            Assert.AreEqual(1, dummy.ReceiveValue(0, 0)); // Does not impact program
        }

        [TestMethod]
        public void SendValue_SecondMB()
        {
            _config = new(4, 4);
            var dummy = Create();

            dummy.PostValue(1, 0, 4);
            Assert.AreEqual(1, dummy.ReceiveValue(0, 0));
            Assert.AreEqual(4, dummy.ReceiveValue(1, 0));
            Assert.AreEqual(1, dummy.ReceiveValue(1, 1));

            dummy.PostValue(1, 1, 3);
            Assert.AreEqual(1, dummy.ReceiveValue(0, 0));
            Assert.AreEqual(4, dummy.ReceiveValue(1, 0));
            Assert.AreEqual(3, dummy.ReceiveValue(1, 1));
        }

        [TestMethod]
        [DataRow(1, 1, 4)]
        [DataRow(0, 0, 2)]
        public void SendValue_TriggersBusChangeCallback(int mixBlock, int bus, int newValue)
        {
            SwitcherBusChangeInfo? info = null;

            _config = new(4, 4);
            var dummy = Create();

            dummy.SetOnBusChangeFinishCall(i => info = i);
            dummy.PostValue(mixBlock, bus, newValue);

            Assert.IsNotNull(info);
            Assert.IsTrue(info.Value.IsBusKnown);
            Assert.AreEqual(mixBlock, info.Value.MixBlock);
            Assert.AreEqual(bus, info.Value.Bus);
            Assert.AreEqual(newValue, info.Value.NewValue);
        }

        [TestMethod]
        public void Dispose_DoesNotThrow() => Create().Dispose();

        [TestMethod]
        public void IsConnected() => Assert.IsTrue(Create().IsConnected);

        [TestMethod]
        public async Task Connect_NoException() => await Create().ConnectAsync();

        [TestMethod]
        public async Task Disconnect_NoException() => await Create().DisconnectAsync();
    }
}