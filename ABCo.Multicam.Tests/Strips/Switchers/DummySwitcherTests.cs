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
            Assert.AreEqual(SwitcherBusInputType.ProgramPreview, specs.MixBlocks[0].NativeType);
            Assert.AreEqual(4, specs.MixBlocks[0].ProgramInputs.Count);

            Assert.AreEqual(1, specs.MixBlocks[0].ProgramInputs[0].Id);
            Assert.AreEqual(2, specs.MixBlocks[0].ProgramInputs[1].Id);
            Assert.AreEqual(3, specs.MixBlocks[0].ProgramInputs[2].Id);
            Assert.AreEqual(4, specs.MixBlocks[0].ProgramInputs[3].Id);

            Assert.AreEqual("Cam 1", specs.MixBlocks[0].ProgramInputs[0].Name);
            Assert.AreEqual("Cam 2", specs.MixBlocks[0].ProgramInputs[1].Name);
            Assert.AreEqual("Cam 3", specs.MixBlocks[0].ProgramInputs[2].Name);
            Assert.AreEqual("Cam 4", specs.MixBlocks[0].ProgramInputs[3].Name);

            Assert.AreEqual(specs.MixBlocks[0].ProgramInputs, specs.MixBlocks[0].PreviewInputs);
        }
    }
}