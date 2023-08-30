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
            Mock<IPerSpecSwitcherInteractionBuffer> Buffer,
            Mock<IDynamicSwitcherInteractionBuffer> DynamicBuffer,
            Mock<IBinderForSwitcherFeature> UIBinder,
            SwitcherSpecs BufferSpecs,
            Mock<IServiceSource> ServSource);

        Mocks _mocks = new();

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.UIBinder = new();

            _mocks.BufferSpecs = new();
            _mocks.Buffer = Mock.Get(Mock.Of<IPerSpecSwitcherInteractionBuffer>(m => m.Specs == _mocks.BufferSpecs && m.IsConnected == true));

            _mocks.DynamicBuffer = new();
            _mocks.DynamicBuffer.SetupGet(m => m.CurrentBuffer).Returns(_mocks.Buffer.Object);

            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<IBinderForSwitcherFeature, ISwitcherRunningFeature>(It.IsAny<ISwitcherRunningFeature>())).Returns(_mocks.UIBinder.Object);
            _mocks.ServSource.Setup(m => m.Get<IDynamicSwitcherInteractionBuffer, ISwitcherEventHandler>(It.IsAny<ISwitcherEventHandler>())).Returns(_mocks.DynamicBuffer.Object);
        }

        public SwitcherRunningFeature Create() => new(_mocks.ServSource.Object);

        [TestMethod]
        public void Ctor()
        {
            var feature = Create();
            Assert.IsInstanceOfType(feature.SwitcherConfig, typeof(DummySwitcherConfig));
            _mocks.ServSource.Verify(m => m.Get<IBinderForSwitcherFeature, ISwitcherRunningFeature>(feature), Times.Once);
            _mocks.DynamicBuffer.Verify(m => m.ChangeSwitcher(It.IsAny<DummySwitcherConfig>()), Times.Once);
            _mocks.ServSource.Verify(m => m.Get<IDynamicSwitcherInteractionBuffer, ISwitcherEventHandler>(feature), Times.Once);
            _mocks.UIBinder.Verify(m => m.ModelChange_Config());
        }

        [TestMethod]
        public void FeatureType() => Assert.AreEqual(FeatureTypes.Switcher, Create().FeatureType);

        [TestMethod]
        public void UIBinder() => Assert.AreEqual(_mocks.UIBinder.Object, Create().UIBinder);

        [TestMethod]
        public void GetProgram()
        {
            Create().GetProgram(3);
            _mocks.DynamicBuffer.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.GetProgram(3));
        }

        [TestMethod]
        public void GetPreview()
        {
            Create().GetPreview(3);
            _mocks.DynamicBuffer.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.GetPreview(3));
        }

        [TestMethod]
        public void SendProgram()
        {
            Create().SendProgram(3, 34);
            _mocks.DynamicBuffer.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.SendProgram(3, 34));
        }

        [TestMethod]
        public void SendPreview()
        {
            Create().SendPreview(3, 34);
            _mocks.DynamicBuffer.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.SendPreview(3, 34));
        }

        [TestMethod]
        public void SwitcherSpecs()
        {
            Assert.AreEqual(_mocks.BufferSpecs, Create().SwitcherSpecs);
            _mocks.DynamicBuffer.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.VerifyGet(m => m.Specs);
        }

        [TestMethod]
        public void IsConnected()
        {
            Assert.IsTrue(Create().IsConnected);
            _mocks.DynamicBuffer.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.VerifyGet(m => m.IsConnected);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void Cut(int mixBlock)
        {
            Create().Cut(mixBlock);
            _mocks.DynamicBuffer.VerifyGet(m => m.CurrentBuffer);
            _mocks.Buffer.Verify(m => m.Cut(mixBlock));
        }

        [TestMethod]
        public void ChangeSwitcher()
        {
            var config = new DummySwitcherConfig(4);
            var feature = Create();
            feature.ChangeSwitcher(config);
            Assert.AreEqual(config, feature.SwitcherConfig);

            _mocks.DynamicBuffer.Verify(m => m.ChangeSwitcher(config));
            _mocks.UIBinder.Verify(m => m.ModelChange_Config());
        }

        [TestMethod]
        public void OnProgramChangeFinish()
        {
            var feature = Create();
            feature.OnProgramChangeFinish(new SwitcherProgramChangeInfo());
            _mocks.UIBinder.Verify(m => m.ModelChange_BusValues());
        }

        [TestMethod]
        public void OnPreviewChangeFinish()
        {
            var feature = Create();
            feature.OnPreviewChangeFinish(new SwitcherPreviewChangeInfo());
            _mocks.UIBinder.Verify(m => m.ModelChange_BusValues());
        }

        [TestMethod]
        public void OnSpecsChange()
        {
            var feature = Create();
            feature.OnSpecsChange(new());
            _mocks.UIBinder.Verify(m => m.ModelChange_Specs());
        }

        // TODO: Add test to verify that it also works when switcher is changed to dummy again?
        [TestMethod]
        public void Dispose()
        {
            Create().Dispose();
            _mocks.DynamicBuffer.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
