using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers.Types
{
    [TestClass]
    public class DummySwitcherTests
    {
        public DummySwitcher Create() => new DummySwitcher();

        [TestMethod]
        public void Ctor_CorrectDefaults()
        {
            var dummy = Create();
            var specs = dummy.ReceiveSpecs();

            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(4, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_ZeroMixBlocks()
        {
            var dummy = Create();
            dummy.UpdateSpecs(Array.Empty<DummyMixBlock>());
            Assert.AreEqual(0, dummy.ReceiveSpecs().MixBlocks.Count);
        }

        [TestMethod]
        public void UpdateSpecs_ZeroInputMixBlock()
        {
            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(0, SwitcherMixBlockType.ProgramPreview) });

            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(0, specs.MixBlocks[0].ProgramInputs.Count);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_MixBlock_ProgramPreview()
        {
            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(1, SwitcherMixBlockType.ProgramPreview) });

            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(1, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_MixBlock_CutBus()
        {
            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(1, SwitcherMixBlockType.CutBus) });

            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.CutBus, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(1, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            Assert.IsNull(specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_TwoMixBlocks_PreviewProgram()
        {
            var dummy = Create();

            dummy.UpdateSpecs(new DummyMixBlock[]
            {
                new DummyMixBlock(2, SwitcherMixBlockType.ProgramPreview),
                new DummyMixBlock(2, SwitcherMixBlockType.ProgramPreview),
            });

            var specs = dummy.ReceiveSpecs();

            Assert.AreEqual(2, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(SwitcherMixBlockType.ProgramPreview, specs.MixBlocks[1].NativeType);

            // Mix Block 1
            Assert.AreEqual(2, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);

            // Mix Block 2
            Assert.AreEqual(2, specs.MixBlocks[1].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[1].ProgramInputs);
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

        [TestMethod]
        public void ReceiveSpecs_MatchesRegular()
        {
            var dummy = Create();
            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(dummy.ReceiveSpecs(), specs);
        }

        [TestMethod]
        public void ReceiveValue_Program_Default()
        {
            Assert.AreEqual(1, Create().ReceiveValue(0, 0));
        }

        [TestMethod]
        public void ReceiveValue_Preview_Default()
        {
            Assert.AreEqual(1, Create().ReceiveValue(0, 1));
        }

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
        public void ReceiveAndSendValue_InvalidBus_CutBus()
        {
            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new(4, SwitcherMixBlockType.CutBus) });
            Assert.ThrowsException<ArgumentException>(() => dummy.ReceiveValue(0, 1));
            Assert.ThrowsException<ArgumentException>(() => dummy.ReceiveValue(0, -1));
            Assert.ThrowsException<ArgumentException>(() => dummy.ReceiveValue(0, 2));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, 1, 3));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, -1, 3));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, 2, 3));
        }

        [TestMethod]
        public void SendValue_InvalidInput()
        {
            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(2, SwitcherMixBlockType.ProgramPreview) });
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, 0, -1));
            Assert.ThrowsException<ArgumentException>(() => dummy.PostValue(0, 0, 3));
        }

        [TestMethod]
        public void SendValue_PreviewProgramMB_Program()
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
        public void SendValue_PreviewProgramMB_Preview()
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
        public void SendValue_CutBusMB()
        {
            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new(3, SwitcherMixBlockType.CutBus) });

            dummy.PostValue(0, 0, 2);
            Assert.AreEqual(2, dummy.ReceiveValue(0, 0));

            dummy.PostValue(0, 0, 3);
            Assert.AreEqual(3, dummy.ReceiveValue(0, 0));
        }

        [TestMethod]
        public void SendValue_SecondMB()
        {
            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new(4, SwitcherMixBlockType.CutBus), new(4, SwitcherMixBlockType.ProgramPreview) });

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
        public void SendValue_TriggersBusChangeCallback()
        {
            SwitcherBusChangeInfo? info = null;

            var dummy = Create();
            dummy.UpdateSpecs(new DummyMixBlock[] { new(4, SwitcherMixBlockType.CutBus), new(4, SwitcherMixBlockType.ProgramPreview) });

            dummy.SetOnBusChangeCallback(i => info = i);
            dummy.PostValue(1, 1, 4);

            Assert.IsNotNull(info);
            Assert.IsTrue(info.Value.IsBusKnown);
            Assert.AreEqual(1, info.Value.MixBlock);
            Assert.AreEqual(1, info.Value.Bus);
            Assert.AreEqual(4, info.Value.NewValue);
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