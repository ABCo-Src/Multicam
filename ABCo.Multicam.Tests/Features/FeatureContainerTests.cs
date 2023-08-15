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
            Mock<IFeatureManager> FeatureManager,
            Mock<ISwitcherRunningFeature> SwitcherFeature,
            Mock<IBinderForFeatureContainer> UIBinder,
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
            _mocks.ServSource.Setup(m => m.Get<IUnsupportedRunningFeature>()).Returns(() => _mocks.UnsupportedFeature.Object);
        }

        public FeatureContainer Create()
        {
            var container = new FeatureContainer(_mocks.UIBinder.Object, _mocks.ServSource.Object, _mocks.FeatureManager.Object);
            container.FinishConstruction(_type);
            return container;
        }

        [TestMethod]
        public void Create_InitUIBinder()
        {
            var container = Create();
            _mocks.UIBinder.Verify(m => m.FinishConstruction(_mocks.FeatureManager.Object, container));
        }

        [TestMethod]
        public void Create_Switcher()
        {
            _type = FeatureTypes.Switcher;
            var manager = Create();
            _mocks.ServSource.Verify(m => m.Get<ISwitcherRunningFeature>(), Times.Once);
            Assert.AreEqual(_mocks.SwitcherFeature.Object, manager.CurrentFeature);
        }

        [TestMethod]
        public void Create_Unsupported()
        {
            _type = FeatureTypes.Unsupported;
            var manager = Create();
            _mocks.ServSource.Verify(m => m.Get<IUnsupportedRunningFeature>(), Times.Once);
            Assert.AreEqual(_mocks.UnsupportedFeature.Object, manager.CurrentFeature);
        }

        [TestMethod]
        public void UIBinder()
        {
            var manager = Create();
            Assert.AreEqual(_mocks.UIBinder.Object, manager.UIBinder);
        }

        //[TestMethod]
        //[DataRow(FeatureTypes.Switcher)]
        //[DataRow(FeatureTypes.Unsupported)]
        //public void FeatureType(FeatureTypes type)
        //{
        //    _type = type;
        //    Assert.AreEqual(type, Create().Type);
        //}

        [TestMethod]
        public void Dispose()
        {
            _type = FeatureTypes.Switcher;
            Create().Dispose();
            _mocks.SwitcherFeature.Verify(m => m.Dispose());
        }
    }
}
