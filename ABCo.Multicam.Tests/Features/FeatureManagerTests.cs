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
        static FeatureManager CreateDefault()
        {
            var mock = new Mock<IServiceSource>();
            mock.Setup(m => m.Get<IUnsupportedRunningFeature>()).Returns(() => Mock.Of<IUnsupportedRunningFeature>());
            return new FeatureManager(mock.Object);
        }

        static FeatureManager CreateWithCustomSource(IServiceSource src) => new FeatureManager(src);

        [TestMethod]
        public void Ctor()
        {
            var manager = CreateDefault();
            Assert.IsNotNull(manager.Features);
        }

        [TestMethod]
        public void CreateFeature_AddsFeature()
        {
            var manager = CreateDefault();
            manager.CreateFeature(FeatureTypes.Unsupported);
            Assert.AreEqual(1, manager.Features.Count);
        }

        [TestMethod]
        public void CreateFeature_Switcher() => TestAddFeatureType<ISwitcherRunningFeature>(FeatureTypes.Switcher);

        [TestMethod]
        public void CreateFeature_Unsupported() => TestAddFeatureType<IUnsupportedRunningFeature>(FeatureTypes.Tally);

        private static void TestAddFeatureType<T>(FeatureTypes type) where T : class
        {
            var mock = new Mock<IServiceSource>();
            mock.Setup(m => m.Get<T>()).Returns(Mock.Of<T>());

            var manager = CreateWithCustomSource(mock.Object);
            manager.CreateFeature(type);

            mock.Verify(m => m.Get<T>(), Times.Once);
            Assert.IsTrue(manager.Features[0].GetType().IsAssignableTo(typeof(T)));
        }

        [TestMethod]
        public void MoveDown_OneItem()
        {
            var manager = CreateDefault();
            manager.CreateFeature(FeatureTypes.Unsupported);
            var newFeature = manager.Features[0];

            manager.MoveDown(newFeature);

            Assert.AreEqual(newFeature, manager.Features[0]);
        }

        [TestMethod]
        public void MoveDown_OnTop()
        {
            var manager = CreateDefault();
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
            var manager = CreateDefault();
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
            var manager = CreateDefault();
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
            var manager = CreateDefault();
            manager.CreateFeature(FeatureTypes.Unsupported);
            var newFeature = manager.Features[0];

            manager.MoveUp(newFeature);

            Assert.AreEqual(newFeature, manager.Features[0]);
        }

        [TestMethod]
        public void MoveUp_OnTop()
        {
            var manager = CreateDefault();
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
            var manager = CreateDefault();
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
            var manager = CreateDefault();
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
            var manager = CreateDefault();
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
            var mockFeature = new Mock<IUnsupportedRunningFeature>();
            var serviceSource = Mock.Of<IServiceSource>(s => s.Get<IUnsupportedRunningFeature>() == mockFeature.Object);
            var manager = CreateWithCustomSource(serviceSource);

            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.Delete(mockFeature.Object);

            mockFeature.Verify(m => m.Dispose());
        }

        [TestMethod]
        public void CreateFeature_Trigger()
        {
            bool triggered = false;
            var manager = CreateDefault();
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
            var manager = CreateDefault();
            manager.CreateFeature(FeatureTypes.Unsupported);

            manager.SetOnFeaturesChangeForVM(() => triggered = true);

            manager.Delete(manager.Features[0]);
            Assert.IsTrue(triggered);
        }

        [TestMethod]
        public void Dispose_DisposesAllFeatures()
        {
            var mockFeature1 = new Mock<IUnsupportedRunningFeature>();
            var mockFeature2 = new Mock<ISwitcherRunningFeature>();
            var serviceSource = Mock.Of<IServiceSource>(s => s.Get<IUnsupportedRunningFeature>() == mockFeature1.Object && s.Get<ISwitcherRunningFeature>() == mockFeature2.Object);
            var manager = CreateWithCustomSource(serviceSource);

            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Switcher);
            manager.Dispose();

            mockFeature1.Verify(m => m.Dispose());
            mockFeature2.Verify(m => m.Dispose());
        }

        // TODO: Add a sanity check to this function that verifies something *did* change
        static void TestTriggerForSingleOperation(Action<FeatureManager> op, bool needed)
        {
            bool triggered = false;
            var manager = CreateDefault();

            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.CreateFeature(FeatureTypes.Unsupported);
            manager.SetOnFeaturesChangeForVM(() => triggered = true);

            op(manager);

            Assert.AreEqual(needed, triggered);
        }
    }
}
