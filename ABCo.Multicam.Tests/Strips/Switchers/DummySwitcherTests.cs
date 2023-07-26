using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Strips.Switchers
{
    [TestClass]
    public class DummySwitcherTests
    {
        public DummySwitcher CreateDefault() => new DummySwitcher();

        [TestMethod]
        public void Ctor_CorrectDefaults()
        {
            var dummy = CreateDefault();
            var specs = dummy.ReceiveSpecs();

            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockInputType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(4, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_ZeroMixBlocks()
        {
            var dummy = CreateDefault();
            dummy.UpdateSpecs(Array.Empty<DummyMixBlock>());
            Assert.AreEqual(0, dummy.ReceiveSpecs().MixBlocks.Count);
        }

        [TestMethod]
        public void UpdateSpecs_ZeroInputMixBlock()
        {
            var dummy = CreateDefault();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(0, SwitcherMixBlockInputType.ProgramPreview) });

            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockInputType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(0, specs.MixBlocks[0].ProgramInputs.Count);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_MixBlock_ProgramPreview()
        {
            var dummy = CreateDefault();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(1, SwitcherMixBlockInputType.ProgramPreview) });

            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockInputType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(1, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_MixBlock_CutBus()
        {
            var dummy = CreateDefault();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(1, SwitcherMixBlockInputType.CutBus) });

            var specs = dummy.ReceiveSpecs();
            Assert.AreEqual(1, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockInputType.CutBus, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(1, specs.MixBlocks[0].ProgramInputs.Count);
            AssertInputsList(specs.MixBlocks[0].ProgramInputs);
            Assert.IsNull(specs.MixBlocks[0].PreviewInputs);
        }

        [TestMethod]
        public void UpdateSpecs_TwoMixBlocks_PreviewProgram()
        {
            var dummy = CreateDefault();

            dummy.UpdateSpecs(new DummyMixBlock[]
            {
                new DummyMixBlock(2, SwitcherMixBlockInputType.ProgramPreview),
                new DummyMixBlock(2, SwitcherMixBlockInputType.ProgramPreview),
            });

            var specs = dummy.ReceiveSpecs();

            Assert.AreEqual(2, specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherMixBlockInputType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(SwitcherMixBlockInputType.ProgramPreview, specs.MixBlocks[1].NativeType);

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
        public async Task ReceiveSpecsAsync_MatchesRegular()
        {
            var dummy = CreateDefault();
            var specs = await dummy.ReceiveSpecsAsync();
            Assert.AreEqual(dummy.ReceiveSpecs(), specs);
        }

        [TestMethod]
        public async Task ReceiveValueAsync_Program_Default()
        {
            var value = await CreateDefault().ReceiveValueAsync(0, 0);
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public async Task ReceiveValueAsync_Preview_Default()
        {
            var value = await CreateDefault().ReceiveValueAsync(0, 1);
            Assert.AreEqual(1, value);
        }


        [TestMethod]
        public async Task ReceiveAndSendValue_InvalidMixBlock()
        {
            var dummy = CreateDefault();
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.ReceiveValueAsync(1, 0));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.ReceiveValueAsync(-1, 0));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(1, 0, 3));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(-1, 0, 3));
        }

        [TestMethod]
        public async Task ReceiveAndSendValue_InvalidBus_PreviewProgram()
        {
            var dummy = CreateDefault();
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.ReceiveValueAsync(0, -1));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.ReceiveValueAsync(0, 2));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(0, -1, 3));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(0, 2, 3));
        }

        [TestMethod]
        public async Task ReceiveAndSendValue_InvalidBus_CutBus()
        {
            var dummy = CreateDefault();
            dummy.UpdateSpecs(new DummyMixBlock[] { new(4, SwitcherMixBlockInputType.CutBus) });
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.ReceiveValueAsync(0, 1));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.ReceiveValueAsync(0, -1));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.ReceiveValueAsync(0, 2));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(0, 1, 3));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(0, -1, 3));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(0, 2, 3));
        }

        [TestMethod]
        public async Task SendValue_InvalidInput()
        {
            var dummy = CreateDefault();
            dummy.UpdateSpecs(new DummyMixBlock[] { new DummyMixBlock(2, SwitcherMixBlockInputType.ProgramPreview) });
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(0, 0, -1));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dummy.SendValueAsync(0, 0, 3));
        }

        [TestMethod]
        public async Task SendValue_PreviewProgramMB_Program()
        {
            var dummy = CreateDefault();

            await dummy.SendValueAsync(0, 0, 2);
            Assert.AreEqual(2, await dummy.ReceiveValueAsync(0, 0)); // Impacts program
            Assert.AreEqual(1, await dummy.ReceiveValueAsync(0, 1)); // Does not impact preview

            await dummy.SendValueAsync(0, 0, 3);
            Assert.AreEqual(3, await dummy.ReceiveValueAsync(0, 0)); // Impacts program
            Assert.AreEqual(1, await dummy.ReceiveValueAsync(0, 1)); // Does not impact preview
        }

        [TestMethod]
        public async Task SendValue_PreviewProgramMB_Preview()
        {
            var dummy = CreateDefault();

            await dummy.SendValueAsync(0, 1, 2);
            Assert.AreEqual(2, await dummy.ReceiveValueAsync(0, 1)); // Impacts preview
            Assert.AreEqual(1, await dummy.ReceiveValueAsync(0, 0)); // Does not impact program

            await dummy.SendValueAsync(0, 1, 3);
            Assert.AreEqual(3, await dummy.ReceiveValueAsync(0, 1)); // Impacts preview
            Assert.AreEqual(1, await dummy.ReceiveValueAsync(0, 0)); // Does not impact program
        }

        [TestMethod]
        public void Dispose_DoesNotThrow() => CreateDefault().Dispose();
    }
}