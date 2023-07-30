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
        public record struct Mocks(
            Mock<ISwitcherRunningFeature> Feature,
            Mock<IProjectFeaturesViewModel> Parent,
            Mock<ISwitcherMixBlockVM>[] MixBlocks,
            Mock<ISwitcherMixBlockVM> SecondMixBlockMock,
            Mock<IServiceSource> ServiceSource);

        Action _onBusChangeCallback = () => { };
        int _currentMixBlockVM = 0;
        SwitcherSpecs _featureSpecs = new();
        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _currentMixBlockVM = 0;
            _featureSpecs = new SwitcherSpecs(new SwitcherMixBlock[]
            {
                SwitcherMixBlock.NewCutBus(),
                SwitcherMixBlock.NewProgPrev()
            });

            _mocks.Feature = new Mock<ISwitcherRunningFeature>();
            _mocks.Feature.Setup(s => s.SwitcherSpecs).Returns(() => _featureSpecs);
            _mocks.Feature.Setup(f => f.SetOnBusChangeForVM(It.IsAny<Action>())).Callback<Action>(v => _onBusChangeCallback = v);
            _mocks.Feature.Setup(f => f.GetValue(0, 0)).Returns(1);
            _mocks.Feature.Setup(f => f.GetValue(0, 1)).Returns(2);
            _mocks.Feature.Setup(f => f.GetValue(1, 0)).Returns(3);
            _mocks.Feature.Setup(f => f.GetValue(1, 1)).Returns(4);
            _mocks.Parent = new Mock<IProjectFeaturesViewModel>();
            _mocks.MixBlocks = new Mock<ISwitcherMixBlockVM>[] { new(), new() };
            _mocks.ServiceSource = new Mock<IServiceSource>();
            _mocks.ServiceSource
                .Setup(s => s.GetVM<ISwitcherMixBlockVM>(It.IsAny<NewViewModelInfo>())).Returns(() => _mocks.MixBlocks[_currentMixBlockVM++].Object);
        }

        public SwitcherFeatureViewModel Create() => new(new(_mocks.Feature.Object, _mocks.Parent.Object), _mocks.ServiceSource.Object);

        [TestMethod]
        public void CtorAndRunningFeature()
        {
            var vm = Create();

            Assert.AreEqual(_mocks.Parent.Object, vm.Parent);
            Assert.AreEqual(_mocks.Feature.Object, vm.BaseFeature);
            Assert.IsNotNull(vm.MixBlocks);
        }

        [TestMethod]
        public void Ctor_SetsVMToMatchSpecs()
        {
            var vm = Create();

            Assert.AreEqual(2, vm.MixBlocks.Count);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherMixBlockVM>(new(_featureSpecs.MixBlocks[0], vm)), Times.Once);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherMixBlockVM>(new(_featureSpecs.MixBlocks[1], vm)), Times.Once);

            Assert.AreEqual(_mocks.MixBlocks[0].Object, vm.MixBlocks[0]);
            Assert.AreEqual(_mocks.MixBlocks[1].Object, vm.MixBlocks[1]);
        }

        [TestMethod]
        public void Ctor_UpdatesMixBlocks()
        {
            var vm = Create();
            _mocks.MixBlocks[0].Verify(m => m.UpdateValue(1, 2));
            _mocks.MixBlocks[1].Verify(m => m.UpdateValue(3, 4));
        }

        [TestMethod]
        public void OnBusChange_UpdatesMixBlocks()
        {
            var vm = Create();

            _mocks.Feature.Setup(f => f.GetValue(0, 0)).Returns(5);
            _mocks.Feature.Setup(f => f.GetValue(0, 1)).Returns(6);
            _mocks.Feature.Setup(f => f.GetValue(1, 0)).Returns(7);
            _mocks.Feature.Setup(f => f.GetValue(1, 1)).Returns(8);
            _onBusChangeCallback();

            _mocks.MixBlocks[0].Verify(m => m.UpdateValue(5, 6));
            _mocks.MixBlocks[1].Verify(m => m.UpdateValue(7, 8));
        }

        [TestMethod]
        public void ContentView()
        {
            var vm = Create();
            Assert.AreEqual(FeatureViewType.Switcher, vm.ContentView);
        }
    }
}
