using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using Moq;

namespace ABCo.Multicam.Tests.Features.Switchers
{
    [TestClass]
    public class SwitcherRunningFeatureTests
    {
        public record struct Mocks(
            Mock<ISwitcherInteractionBuffer> Buffer,
            Mock<ISwitcherSwapper> Swapper,
            Mock<IBinderForSwitcherFeature> UIBinder,
            SwitcherSpecs BufferSpecs,
            Mock<IServiceSource> ServSource);

        Mocks _mocks = new();

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.UIBinder = new();

            _mocks.BufferSpecs = new();
            _mocks.Buffer = Mock.Get(Mock.Of<ISwitcherInteractionBuffer>(m => m.Specs == _mocks.BufferSpecs && m.IsConnected == true));

            _mocks.Swapper = new();
            _mocks.Swapper.SetupGet(m => m.CurrentBuffer).Returns(_mocks.Buffer.Object);

            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<IBinderForSwitcherFeature, ISwitcherRunningFeature>(It.IsAny<ISwitcherRunningFeature>())).Returns(_mocks.UIBinder.Object);
            _mocks.ServSource.Setup(m => m.Get<ISwitcherSwapper, ISwitcherEventHandler>(It.IsAny<ISwitcherEventHandler>())).Returns(_mocks.Swapper.Object);
        }

        public SwitcherRunningFeature Create() => new(_mocks.ServSource.Object);

        [TestMethod]
        public void Ctor()
        {
            var feature = Create();
            _mocks.ServSource.Verify(m => m.Get<IBinderForSwitcherFeature, ISwitcherRunningFeature>(feature), Times.Once);
            _mocks.ServSource.Verify(m => m.Get<ISwitcherSwapper, ISwitcherEventHandler>(feature), Times.Once);
        }

        [TestMethod]
        public void FeatureType() => Assert.AreEqual(FeatureTypes.Switcher, Create().FeatureType);

        [TestMethod]
        public void UIBinder() => Assert.AreEqual(_mocks.UIBinder.Object, Create().UIBinder);

        [TestMethod]
        public void GetValue()
        {
            Create().GetValue(3, 8);
            _mocks.Swapper.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.GetValue(3, 8));
        }

        [TestMethod]
        public void PostValue()
        {
            Create().PostValue(3, 7, 34);
            _mocks.Swapper.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.PostValue(3, 7, 34));
        }

        [TestMethod]
        public void SwitcherSpecs()
        {
            Assert.AreEqual(_mocks.BufferSpecs, Create().SwitcherSpecs);
            _mocks.Swapper.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.VerifyGet(m => m.Specs);
        }

        [TestMethod]
        public void IsConnected()
        {
            Assert.IsTrue(Create().IsConnected);
            _mocks.Swapper.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.VerifyGet(m => m.IsConnected);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void Cut(int mixBlock)
        {
            Create().Cut(mixBlock);
            _mocks.Swapper.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.Cut(mixBlock));
        }

        [TestMethod]
        public void ChangeSwitcher()
        {
            var config = new DummySwitcherConfig(4);
            Create().ChangeSwitcher(config);
            _mocks.Swapper.Verify(m => m.ChangeSwitcher(config));
        }

        [TestMethod]
        public void OnBusChange()
        {
            var feature = Create();
            feature.OnBusChangeFinish(new RetrospectiveFadeInfo());
            _mocks.UIBinder.Verify(m => m.ModelChange_BusValues());
        }

        // TODO: Add test to verify that it also works when switcher is changed to dummy again?
        [TestMethod]
        public void Dispose()
        {
            Create().Dispose();
            _mocks.Swapper.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
