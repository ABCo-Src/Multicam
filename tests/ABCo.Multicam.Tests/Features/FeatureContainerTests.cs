using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using Moq;

namespace ABCo.Multicam.Tests.Features
{
	[TestClass]
    public class FeatureContainerTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServSource,
            Mock<IFeatureManager> FeatureManager,
            Mock<ISwitcherRunningFeature> SwitcherFeature,
            Mock<IGeneralFeaturePresenter> UIBinder,
            Mock<IUnsupportedRunningFeature> UnsupportedFeature
        );

        Mocks _mocks = new();
        FeatureTypes _type;

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.SwitcherFeature = new();
            _mocks.FeatureManager = new();
            _mocks.UnsupportedFeature = new();
            _mocks.UIBinder = new();

            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<ISwitcherRunningFeature>()).Returns(() => _mocks.SwitcherFeature.Object);
            _mocks.ServSource.Setup(m => m.Get<IGeneralFeaturePresenter, IFeatureManager, IFeature>(It.IsAny<IFeatureManager>(), It.IsAny<IFeature>()))
                .Returns(_mocks.UIBinder.Object)
                .Callback<IFeatureManager, IFeature>((m, c) => Assert.IsNotNull(m));

            _mocks.ServSource.Setup(m => m.Get<IUnsupportedRunningFeature>()).Returns(() => _mocks.UnsupportedFeature.Object);
        }

        public Feature Create()
        {
            var container = new Feature(_mocks.ServSource.Object, _mocks.FeatureManager.Object);
            container.FinishConstruction(_type);
            return container;
        }

        [TestMethod]
        public void Create_InitUIBinder()
        {
            var container = Create();
            _mocks.ServSource.Verify(m => m.Get<IGeneralFeaturePresenter, IFeatureManager, IFeature>(_mocks.FeatureManager.Object, container));
        }

        [TestMethod]
        public void Create_Switcher()
        {
            _type = FeatureTypes.Switcher;
            var manager = Create();
            _mocks.ServSource.Verify(m => m.Get<ISwitcherRunningFeature>(), Times.Once);
            Assert.AreEqual(_mocks.SwitcherFeature.Object, manager.LiveFeature);
        }

        [TestMethod]
        public void Create_Unsupported()
        {
            _type = FeatureTypes.Unsupported;
            var manager = Create();
            _mocks.ServSource.Verify(m => m.Get<IUnsupportedRunningFeature>(), Times.Once);
            Assert.AreEqual(_mocks.UnsupportedFeature.Object, manager.LiveFeature);
        }

        [TestMethod]
        public void UIBinder()
        {
            var manager = Create();
            Assert.AreEqual(_mocks.UIBinder.Object, manager.UIPresenter);
        }

        [TestMethod]
        public void Dispose()
        {
            _type = FeatureTypes.Switcher;
            Create().Dispose();
            _mocks.SwitcherFeature.Verify(m => m.Dispose());
        }
    }
}
