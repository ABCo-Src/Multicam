using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features
{
    [TestClass]
    public class FeatureContainerTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServSource,
            Mock<ISwitcherRunningFeature> SwitcherFeature,
            Mock<IUnsupportedRunningFeature> UnsupportedFeature
        );

        Mocks _mocks = new();
        FeatureTypes _type;

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.SwitcherFeature = new();
            _mocks.UnsupportedFeature = new();

            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<ISwitcherRunningFeature>()).Returns(() => _mocks.SwitcherFeature.Object);
            _mocks.ServSource.Setup(m => m.Get<IUnsupportedRunningFeature>()).Returns(() => _mocks.UnsupportedFeature.Object);
        }

        public FeatureContainer Create()
        {
            var container = new FeatureContainer(_mocks.ServSource.Object);
            container.FinishConstruction(_type);
            return container;
        }

        [TestMethod]
        public void Create_Switcher()
        {
            _type = FeatureTypes.Switcher;
            var manager = Create();
            _mocks.ServSource.Verify(m => m.Get<ISwitcherRunningFeature>(), Times.Once);
        }

        [TestMethod]
        public void Create_Unsupported()
        {
            _type = FeatureTypes.Unsupported;
            var manager = Create();
            _mocks.ServSource.Verify(m => m.Get<IUnsupportedRunningFeature>(), Times.Once);
        }
    }
}
