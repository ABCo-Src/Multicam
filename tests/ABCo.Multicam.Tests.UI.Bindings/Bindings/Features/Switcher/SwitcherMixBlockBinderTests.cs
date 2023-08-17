using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using Moq;

namespace ABCo.Multicam.Tests.UI.Bindings.Features.Switcher
{
    [TestClass]
    public class SwitcherMixBlockBinderTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServSource,
            Mock<ISwitcherRunningFeature> RunningFeature,
            Mock<IVMForSwitcherMixBlock> VM
        );

        SwitcherMixBlock _mixBlock = null!;
        Mocks _mocks;

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.RunningFeature = new();
            _mocks.ServSource = new();
            _mocks.VM = new();
            _mixBlock = new();
        }

        public MixBlockVMBinder Create()
        {
            var binder = new MixBlockVMBinder(_mocks.ServSource.Object);
            binder.FinishConstruction(_mocks.RunningFeature.Object, _mixBlock, 7);
            return binder;
        }

        //[TestMethod]
        //public void RawFeature()
        //{
        //    var binder = Create();
        //    binder.Properties[0].UpdateCache();
        //    binder.Properties[0].UpdateVM(_mocks.VM.Object);
        //    _mocks.VM.VerifySet(m => m.RawFeature = _mocks.RunningFeature.Object);
        //}

        //[TestMethod]
        //public void RawMixBlock()
        //{
        //    var binder = Create();
        //    binder.Properties[1].UpdateCache();
        //    binder.Properties[1].UpdateVM(_mocks.VM.Object);
        //    _mocks.VM.VerifySet(m => m.RawFeature = _mocks.RunningFeature.Object);
        //}
    }
}