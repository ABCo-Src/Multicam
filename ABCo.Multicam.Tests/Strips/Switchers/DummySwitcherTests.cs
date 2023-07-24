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
            var specs = dummy.Specs;

            Assert.AreEqual(1, dummy.Specs.MixBlocks.Count);
            Assert.AreEqual(SwitcherBusInputType.PreviewProgram, dummy.Specs.MixBlocks[0].NativeType);
            Assert.AreEqual(4, dummy.Specs.MixBlocks[0].ProgramInputs.Count);
            Assert.AreEqual(1, dummy.Specs.MixBlocks[0].ProgramInputs[0].Id);
            Assert.AreEqual(2, dummy.Specs.MixBlocks[0].ProgramInputs[1].Id);
            Assert.AreEqual(3, dummy.Specs.MixBlocks[0].ProgramInputs[2].Id);
            Assert.AreEqual(4, dummy.Specs.MixBlocks[0].ProgramInputs[3].Id);
            Assert.AreEqual(dummy.Specs.MixBlocks[0].ProgramInputs, dummy.Specs.MixBlocks[0].PreviewInputs);
        }
    }
}
