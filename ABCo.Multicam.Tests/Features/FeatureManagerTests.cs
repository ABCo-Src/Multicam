using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features
{
    [TestClass]
    public class FeatureManagerTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServiceSource,
            Mock<IUnsupportedRunningFeature> UnsupportedFeature1,
            Mock<IUnsupportedRunningFeature>[] UnsupportedFeatures,
            Mock<ISwitcherRunningFeature> SwitcherFeature
        );

        int _unsupposedFeaturePos = 0;
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _unsupposedFeaturePos = 0;

            _mocks.UnsupportedFeature1 = new();
            _mocks.UnsupportedFeatures = new Mock<IUnsupportedRunningFeature>[] { _mocks.UnsupportedFeature1, new(), new() };
            _mocks.SwitcherFeature = new();

            _mocks.ServiceSource = new();
            _mocks.ServiceSource.Setup(m => m.Get<IUnsupportedRunningFeature>()).Returns(() => _mocks.UnsupportedFeatures[_unsupposedFeaturePos++].Object);
            _mocks.ServiceSource.Setup(m => m.Get<ISwitcherRunningFeature>()).Returns(() => _mocks.SwitcherFeature.Object);
        }

        FeatureManager Create() => new(_mocks.ServiceSource.Object);

        [TestMethod]
        public void Ctor() => Assert.IsNotNull(Create().Features);

        [TestMethod]
        public void CreateFeature_AddsFeature()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            Assert.AreEqual(1, manager.Features.Count);
        }

        [TestMethod]
        public void CreateFeature_Switcher()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Switcher);
            _mocks.ServiceSource.Verify(m => m.Get<ISwitcherRunningFeature>(), Times.Once);
            Assert.AreEqual(_mocks.SwitcherFeature.Object, manager.Features[0]);
        }

        [TestMethod]
        public void CreateFeature_Unsupported()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Tally);
            _mocks.ServiceSource.Verify(m => m.Get<IUnsupportedRunningFeature>(), Times.Once);
            Assert.AreEqual(_mocks.UnsupportedFeature1.Object, manager.Features[0]);
        }

        [TestMethod]
        public void MoveDown_OneItem()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);

            var newFeature = manager.Features[0];
            manager.MoveDown(newFeature);

            Assert.AreEqual(newFeature, manager.Features[0]);
        }

        [TestMethod]
        public void MoveDown_OnTop()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);

            var movingFeature = manager.Features[0];
            var unmovingFeature = manager.Features[1];
            manager.MoveDown(movingFeature);

            Assert.AreEqual(unmovingFeature, manager.Features[0]);
            Assert.AreEqual(movingFeature, manager.Features[1]);
        }

        [TestMethod]
        public void MoveDown_OnBottom()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);

            var movingFeature = manager.Features[1];
            var unmovingFeature = manager.Features[0];
            manager.MoveDown(movingFeature);

            Assert.AreEqual(unmovingFeature, manager.Features[0]);
            Assert.AreEqual(movingFeature, manager.Features[1]);
        }

        [TestMethod]
        public void MoveDown_Middle()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);

            var unmoving1 = manager.Features[0];
            var movingFeature = manager.Features[1];
            var unmoving2 = manager.Features[2];
            manager.MoveDown(movingFeature);

            Assert.AreEqual(unmoving1, manager.Features[0]);
            Assert.AreEqual(unmoving2, manager.Features[1]);
            Assert.AreEqual(movingFeature, manager.Features[2]);
        }

        [TestMethod]
        public void MoveUp_OneItem()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);

            var newFeature = manager.Features[0];
            manager.MoveUp(newFeature);

            Assert.AreEqual(newFeature, manager.Features[0]);
        }

        [TestMethod]
        public void MoveUp_OnTop()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);

            var movingFeature = manager.Features[0];
            var unmovingFeature = manager.Features[1];
            manager.MoveUp(movingFeature);

            Assert.AreEqual(movingFeature, manager.Features[0]);
            Assert.AreEqual(unmovingFeature, manager.Features[1]);
        }

        [TestMethod]
        public void MoveUp_OnBottom()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);

            var movingFeature = manager.Features[1];
            var unmovingFeature = manager.Features[0];
            manager.MoveUp(movingFeature);

            Assert.AreEqual(movingFeature, manager.Features[0]);
            Assert.AreEqual(unmovingFeature, manager.Features[1]);
        }

        [TestMethod]
        public void MoveUp_Middle()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);

            var unmoving1 = manager.Features[0];
            var movingFeature = manager.Features[1];
            var unmoving2 = manager.Features[2];
            manager.MoveUp(movingFeature);

            Assert.AreEqual(movingFeature, manager.Features[0]);
            Assert.AreEqual(unmoving1, manager.Features[1]);
            Assert.AreEqual(unmoving2, manager.Features[2]);
        }

        [TestMethod]
        public void Delete()
        {
            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);

            var unmoving1 = manager.Features[0];
            var movingFeature = manager.Features[1];
            var unmoving2 = manager.Features[2];
            manager.Delete(movingFeature);

            Assert.AreEqual(unmoving1, manager.Features[0]);
            Assert.AreEqual(unmoving2, manager.Features[1]);
        }

        [TestMethod]
        public void Delete_DisposesFeature()
        {
            var manager = Create();

            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.Delete(_mocks.UnsupportedFeature1.Object);

            _mocks.UnsupportedFeature1.Verify(m => m.Dispose());
        }

        [TestMethod]
        public void CreateFeature_Trigger()
        {
            bool triggered = false;

            var manager = Create();
            manager.SetOnFeaturesChangeForVM(() => triggered = true);
            manager.CreateFeature(FeatureTypes.Unsupported);

            Assert.IsTrue(triggered);
        }

        [TestMethod]
        public void MoveDown_Trigger_Unneeded() => TestTriggerForSingleOperation(m => m.MoveDown(m.Features[1]), false);

        [TestMethod]
        public void MoveDown_Trigger_Needed() => TestTriggerForSingleOperation(m => m.MoveDown(m.Features[0]), true);

        [TestMethod]
        public void MoveUp_Trigger_Unneeded() => TestTriggerForSingleOperation(m => m.MoveUp(m.Features[0]), false);

        [TestMethod]
        public void MoveUp_Trigger_Needed() => TestTriggerForSingleOperation(m => m.MoveUp(m.Features[1]), true);

        [TestMethod]
        public void Delete_Trigger()
        {
            bool triggered = false;

            var manager = Create();
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.SetOnFeaturesChangeForVM(() => triggered = true);
            manager.Delete(manager.Features[0]);

            Assert.IsTrue(triggered);
        }

        [TestMethod]
        public void Dispose_DisposesAllFeatures()
        {
            var manager = Create();

            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Switcher);
            manager.Dispose();

            _mocks.SwitcherFeature.Verify(m => m.Dispose());
            _mocks.UnsupportedFeature1.Verify(m => m.Dispose());
        }

        // TODO: Add a sanity check to this function that verifies something *did* change
        void TestTriggerForSingleOperation(Action<FeatureManager> op, bool needed)
        {
            bool triggered = false;
            var manager = Create();

            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.SetOnFeaturesChangeForVM(() => triggered = true);

            op(manager);

            Assert.AreEqual(needed, triggered);
        }
    }
}
