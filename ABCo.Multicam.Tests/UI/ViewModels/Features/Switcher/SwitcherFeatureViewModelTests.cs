using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
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

namespace ABCo.Multicam.Tests.UI.ViewModels.Features.Switcher
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

        Action<RetrospectiveFadeInfo?> _onBusChangeCallback = i => { };
        int _currentMixBlockVM = 0;
        SwitcherSpecs _featureSpecs = new();
        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _currentMixBlockVM = 0;
            _featureSpecs = new SwitcherSpecs(new SwitcherMixBlock[]
            {
                SwitcherMixBlock.NewCutBus(new()),
                SwitcherMixBlock.NewProgPrev(new())
            });

            _mocks.Feature = new Mock<ISwitcherRunningFeature>();
            _mocks.Feature.Setup(s => s.SwitcherSpecs).Returns(() => _featureSpecs);
            _mocks.Feature.Setup(f => f.SetOnBusChangeFinishForVM(It.IsAny<Action<RetrospectiveFadeInfo?>>())).Callback<Action<RetrospectiveFadeInfo?>>(v => _onBusChangeCallback = v);
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
            Assert.IsNotNull(vm.MixBlocks);
        }

        [TestMethod]
        public void Ctor_SetsVMToMatchSpecs()
        {
            var vm = Create();

            Assert.AreEqual(2, vm.MixBlocks.Count);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherMixBlockVM>(new(new MixBlockViewModelInfo(_featureSpecs.MixBlocks[0], 0), vm)), Times.Once);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherMixBlockVM>(new(new MixBlockViewModelInfo(_featureSpecs.MixBlocks[1], 1), vm)), Times.Once);

            Assert.AreEqual(_mocks.MixBlocks[0].Object, vm.MixBlocks[0]);
            Assert.AreEqual(_mocks.MixBlocks[1].Object, vm.MixBlocks[1]);
        }

        [TestMethod]
        public void Ctor_UpdatesMixBlocks()
        {
            var vm = Create();
            _mocks.MixBlocks[0].Verify(m => m.RefreshBuses(1, 2));
            _mocks.MixBlocks[1].Verify(m => m.RefreshBuses(3, 4));
        }

        [TestMethod]
        public void OnBusChange_UpdatesMixBlocks()
        {
            var vm = Create();

            _mocks.Feature.Setup(f => f.GetValue(0, 0)).Returns(5);
            _mocks.Feature.Setup(f => f.GetValue(0, 1)).Returns(6);
            _mocks.Feature.Setup(f => f.GetValue(1, 0)).Returns(7);
            _mocks.Feature.Setup(f => f.GetValue(1, 1)).Returns(8);
            _onBusChangeCallback(null);

            _mocks.MixBlocks[0].Verify(m => m.RefreshBuses(5, 6));
            _mocks.MixBlocks[1].Verify(m => m.RefreshBuses(7, 8));
        }

        //[TestMethod]
        //public void ContentView()
        //{
        //    var vm = Create();
        //    Assert.AreEqual(FeatureViewType.Switcher, vm.ContentView);
        //}

        [TestMethod]
        public void SetValue()
        {
            Create().SetValue(6, 1, 3);
            _mocks.Feature.Verify(m => m.PostValue(6, 1, 3));
        }

        [TestMethod]
        public void Cut()
        {
            Create().Cut(5);
            _mocks.Feature.Verify(m => m.Cut(5));
        }
    }
}
