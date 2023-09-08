using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features.Switcher
{
	[TestClass]
    public class SwitcherFeatureVMTests
    {
        public record struct Mocks(
            Mock<ISwitcherRunningFeature> RawFeature,
            Mock<ISwitcherConfigVM> ConfigVM,
            Mock<IServiceSource> ServSource,
            Mock<IVMBinder<IVMForSwitcherMixBlock>>[] RawMixBlocks,
            Mock<ISwitcherMixBlockVM>[] VMs);

        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _mocks.RawFeature = new();
            _mocks.ConfigVM = new();
            _mocks.RawMixBlocks = new Mock<IVMBinder<IVMForSwitcherMixBlock>>[] { new(), new() };
            _mocks.VMs = new Mock<ISwitcherMixBlockVM>[] { new(), new() };
            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<ISwitcherConfigVM, SwitcherConfig, ISwitcherFeatureVM>(It.IsAny<SwitcherConfig>(), It.IsAny<ISwitcherFeatureVM>())).Returns(_mocks.ConfigVM.Object);

            for (int i = 0; i < 2; i++)
                _mocks.RawMixBlocks[i].Setup(m => m.GetVM<ISwitcherMixBlockVM>(It.IsAny<object>())).Returns(_mocks.VMs[i].Object);
        }

        public SwitcherFeatureVM Create() => new(_mocks.ServSource.Object)
        {
            RawMixBlocks = new[] { _mocks.RawMixBlocks[0].Object, _mocks.RawMixBlocks[1].Object },
            RawFeature = _mocks.RawFeature.Object
        };

        [TestMethod]
        public void UpdateConfig()
        {
            var config = new DummySwitcherConfig();
            Create().UpdateConfig(config);
            _mocks.RawFeature.Verify(m => m.ChangeSwitcher(config));
        }

        [TestMethod]
        public void RawMixBlocks_UpdatesMixBlocks()
        {
            var vm = Create();
            var mixBlocks = vm.MixBlocks!;

            Assert.AreEqual(2, mixBlocks.Length);
            _mocks.RawMixBlocks[0].Verify(m => m.GetVM<ISwitcherMixBlockVM>(vm), Times.Once);
            _mocks.RawMixBlocks[1].Verify(m => m.GetVM<ISwitcherMixBlockVM>(vm), Times.Once);

            Assert.AreEqual(_mocks.VMs[0].Object, mixBlocks[0]);
            Assert.AreEqual(_mocks.VMs[1].Object, mixBlocks[1]);
        }

        [TestMethod]
        public void RawConfig_UpdatesVM()
        {
            var vm = Create();

            var config = new DummySwitcherConfig();
            vm.RawConfig = config;

            Assert.AreEqual(_mocks.ConfigVM.Object, vm.Config);
            _mocks.ServSource.Verify(m => m.Get<ISwitcherConfigVM, SwitcherConfig, ISwitcherFeatureVM>(config, vm));
        }

		[TestMethod]
		public void ToggleConnection_Connected()
		{
			var vm = Create();
			vm.RawIsConnected = true;
			vm.ToggleConnection();
			_mocks.RawFeature.Verify(m => m.Disconnect());
		}

		[TestMethod]
		public void ToggleConnection_Disconnected()
		{
			var vm = Create();
			vm.RawIsConnected = false;
			vm.ToggleConnection();
			_mocks.RawFeature.Verify(m => m.Connect());
		}
	}
}
