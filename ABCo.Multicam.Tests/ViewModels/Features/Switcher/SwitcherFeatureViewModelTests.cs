using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.ViewModels.Features.Switcher
{
    [TestClass]
    public class SwitcherFeatureViewModelTests
    {
        public SwitcherFeatureViewModel CreateDefault() => new(new(Mock.Of<ISwitcherRunningFeature>(s => s.SwitcherSpecs == new SwitcherSpecs()), Mock.Of<IProjectFeaturesViewModel>()), Mock.Of<IServiceSource>());
        public SwitcherFeatureViewModel CreateWithCustomModel(ISwitcherRunningFeature model) => new(new(model, Mock.Of<IProjectFeaturesViewModel>()), Mock.Of<IServiceSource>());
        public SwitcherFeatureViewModel CreateWithCustomModelAndParent(ISwitcherRunningFeature model, IProjectFeaturesViewModel parent) => new(new(model, parent), Mock.Of<IServiceSource>());

        [TestMethod]
        public void CtorAndRunningFeature()
        {
            var model = Mock.Of<ISwitcherRunningFeature>(s => s.SwitcherSpecs == new SwitcherSpecs());
            var parent = Mock.Of<IProjectFeaturesViewModel>();
            var vm = CreateWithCustomModelAndParent(model, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(model, vm.BaseFeature);
            Assert.IsNotNull(vm.MixBlocks);
        }

        [TestMethod]
        public void Ctor_SetsVMToMatchSpecs()
        {
            var testSpecs = new SwitcherSpecs(new SwitcherMixBlock[]
            {
                SwitcherMixBlock.NewCutBus(),
                SwitcherMixBlock.NewProgPrev()
            });

            var model = Mock.Of<ISwitcherRunningFeature>(s => s.SwitcherSpecs == testSpecs);
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
            Assert.AreEqual(FeatureViewType.Switcher, vm.ContentView);
        }
    }
}
