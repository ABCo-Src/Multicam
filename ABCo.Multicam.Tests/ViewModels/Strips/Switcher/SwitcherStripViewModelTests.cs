using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Strips;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Strips.Switcher
{
    [TestClass]
    public class SwitcherStripViewModelTests
    {
        public SwitcherStripViewModel CreateDefault() => new(new(Mock.Of<ISwitcherRunningStrip>(s => s.SwitcherSpecs == new SwitcherSpecs()), Mock.Of<IProjectStripsViewModel>()), Mock.Of<IServiceSource>());
        public SwitcherStripViewModel CreateWithCustomModel(ISwitcherRunningStrip model) => new(new(model, Mock.Of<IProjectStripsViewModel>()), Mock.Of<IServiceSource>());
        public SwitcherStripViewModel CreateWithCustomModelAndParent(ISwitcherRunningStrip model, IProjectStripsViewModel parent) => new(new(model, parent), Mock.Of<IServiceSource>());

        [TestMethod]
        public void CtorAndRunningStrip()
        {
            var model = Mock.Of<ISwitcherRunningStrip>(s => s.SwitcherSpecs == new SwitcherSpecs());
            var parent = Mock.Of<IProjectStripsViewModel>();
            var vm = CreateWithCustomModelAndParent(model, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(model, vm.BaseStrip);
            Assert.IsNotNull(vm.MixBlocks);
        }

        [TestMethod]
        public void Ctor_SetsVMToMatchSpecs()
        {
            var testSpecs = new SwitcherSpecs(new SwitcherMixBlock[]
            {
                // Cut Bus
                new SwitcherMixBlock(SwitcherMixBlockType.CutBus, Array.Empty<SwitcherBusInput>(), null),
                new SwitcherMixBlock(SwitcherMixBlockType.ProgramPreview, Array.Empty<SwitcherBusInput>(), null)
            });

            var model = Mock.Of<ISwitcherRunningStrip>(s => s.SwitcherSpecs == testSpecs);
            var vm = CreateWithCustomModel(model);

            Assert.AreEqual(2, vm.MixBlocks.Count);
            Assert.AreEqual(testSpecs.MixBlocks[0], vm.MixBlocks[0].BaseBlock);
            Assert.AreEqual(testSpecs.MixBlocks[1], vm.MixBlocks[1].BaseBlock);
            Assert.AreEqual(vm, vm.MixBlocks[0].Parent);
        }

        [TestMethod]
        public void ContentView()
        {
            var vm = CreateDefault();
            Assert.AreEqual(StripViewType.Switcher, vm.ContentView);
        }
    }
}
