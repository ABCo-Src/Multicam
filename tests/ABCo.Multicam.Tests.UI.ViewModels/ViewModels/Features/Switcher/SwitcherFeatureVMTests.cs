﻿using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features.Switcher
{
    [TestClass]
    public class SwitcherFeatureVMTests
    {
        public record struct Mocks(
            Mock<IVMBinder<IVMForSwitcherMixBlock>>[] RawMixBlocks,
            Mock<ISwitcherMixBlockVM>[] VMs);

        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _mocks.RawMixBlocks = new Mock<IVMBinder<IVMForSwitcherMixBlock>>[] { new(), new() };
            _mocks.VMs = new Mock<ISwitcherMixBlockVM>[] { new(), new() };

            for (int i = 0; i < 2; i++)
                _mocks.RawMixBlocks[i].Setup(m => m.GetVM<ISwitcherMixBlockVM>(It.IsAny<object>())).Returns(_mocks.VMs[i].Object);
        }

        public SwitcherFeatureVM Create() => new()
        {
            RawMixBlocks = new[] { _mocks.RawMixBlocks[0].Object, _mocks.RawMixBlocks[1].Object }
        };

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
    }
}