﻿//using ABCo.Multicam.Server;
//using ABCo.Multicam.Server.Features;
//using Moq;

//namespace ABCo.Multicam.Tests.Features
//{
//    [TestClass]
//    public class FeatureManagerTests
//    {
//        public record struct Mocks(
//            Mock<IServiceSource> ServiceSource,
//            Mock<IBinderForProjectFeatures> VMBinder,
//            Mock<IFeature>[] Features
//        );

//        Mocks _mocks = new();

//        [TestInitialize]
//        public void InitMocks()
//        {
//            _mocks.Features = new Mock<IFeature>[] { new(), new(), new() };
//            _mocks.VMBinder = new();

//            _mocks.ServiceSource = new();
//            _mocks.ServiceSource.Setup(m => m.Get<IBinderForProjectFeatures, IFeatureManager>(It.IsAny<IFeatureManager>())).Returns(_mocks.VMBinder.Object);
//            _mocks.ServiceSource.SetupSequence(m => m.Get<IFeature, FeatureTypes>(It.IsAny<FeatureTypes>()))
//                .Returns(_mocks.Features[0].Object).Returns(_mocks.Features[1].Object).Returns(_mocks.Features[2].Object);
//        }

//        FeatureManager Create() => new(_mocks.ServiceSource.Object);

//        [TestMethod]
//        public void Ctor()
//        {
//            var model = Create();
//            Assert.IsNotNull(model.Features);
//            Assert.AreEqual(_mocks.VMBinder.Object, model.UIBinder);
//            _mocks.ServiceSource.Verify(m => m.Get<IBinderForProjectFeatures, IFeatureManager>(model));
//        }

//        [TestMethod]
//        [DataRow(FeatureTypes.Unsupported)]
//        [DataRow(FeatureTypes.Switcher)]
//        public void CreateFeature(FeatureTypes type)
//        {
//            var manager = Create();
//            manager.CreateFeature(type);

//            _mocks.ServiceSource.Verify(m => m.Get<IFeature, FeatureTypes>(type));

//            Assert.AreEqual(1, manager.Features.Count);
//            Assert.AreEqual(_mocks.Features[0].Object, manager.Features[0]);
//        }

//        [TestMethod]
//        public void MoveDown_OneItem()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var newFeature = manager.Features[0];
//            manager.MoveDown(newFeature);

//            Assert.AreEqual(newFeature, manager.Features[0]);
//        }

//        [TestMethod]
//        public void MoveDown_OnTop()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var movingFeature = manager.Features[0];
//            var unmovingFeature = manager.Features[1];
//            manager.MoveDown(movingFeature);

//            Assert.AreEqual(unmovingFeature, manager.Features[0]);
//            Assert.AreEqual(movingFeature, manager.Features[1]);
//        }

//        [TestMethod]
//        public void MoveDown_OnBottom()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var movingFeature = manager.Features[1];
//            var unmovingFeature = manager.Features[0];
//            manager.MoveDown(movingFeature);

//            Assert.AreEqual(unmovingFeature, manager.Features[0]);
//            Assert.AreEqual(movingFeature, manager.Features[1]);
//        }

//        [TestMethod]
//        public void MoveDown_Middle()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var unmoving1 = manager.Features[0];
//            var movingFeature = manager.Features[1];
//            var unmoving2 = manager.Features[2];
//            manager.MoveDown(movingFeature);

//            Assert.AreEqual(unmoving1, manager.Features[0]);
//            Assert.AreEqual(unmoving2, manager.Features[1]);
//            Assert.AreEqual(movingFeature, manager.Features[2]);
//        }

//        [TestMethod]
//        public void MoveUp_OneItem()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var newFeature = manager.Features[0];
//            manager.MoveUp(newFeature);

//            Assert.AreEqual(newFeature, manager.Features[0]);
//        }

//        [TestMethod]
//        public void MoveUp_OnTop()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var movingFeature = manager.Features[0];
//            var unmovingFeature = manager.Features[1];
//            manager.MoveUp(movingFeature);

//            Assert.AreEqual(movingFeature, manager.Features[0]);
//            Assert.AreEqual(unmovingFeature, manager.Features[1]);
//        }

//        [TestMethod]
//        public void MoveUp_OnBottom()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var movingFeature = manager.Features[1];
//            var unmovingFeature = manager.Features[0];
//            manager.MoveUp(movingFeature);

//            Assert.AreEqual(movingFeature, manager.Features[0]);
//            Assert.AreEqual(unmovingFeature, manager.Features[1]);
//        }

//        [TestMethod]
//        public void MoveUp_Middle()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var unmoving1 = manager.Features[0];
//            var movingFeature = manager.Features[1];
//            var unmoving2 = manager.Features[2];
//            manager.MoveUp(movingFeature);

//            Assert.AreEqual(movingFeature, manager.Features[0]);
//            Assert.AreEqual(unmoving1, manager.Features[1]);
//            Assert.AreEqual(unmoving2, manager.Features[2]);
//        }

//        [TestMethod]
//        public void Delete()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);

//            var unmoving1 = manager.Features[0];
//            var movingFeature = manager.Features[1];
//            var unmoving2 = manager.Features[2];
//            manager.Delete(movingFeature);

//            Assert.AreEqual(unmoving1, manager.Features[0]);
//            Assert.AreEqual(unmoving2, manager.Features[1]);
//        }

//        [TestMethod]
//        public void Delete_DisposesFeature()
//        {
//            var manager = Create();

//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.Delete(_mocks.Features[0].Object);

//            _mocks.Features[0].Verify(m => m.Dispose());
//        }

//        [TestMethod]
//        public void CreateFeature_Trigger()
//        {
//            Create().CreateFeature(FeatureTypes.Unsupported);
//            _mocks.VMBinder.Verify(m => m.ModelChange_FeaturesChange());
//        }

//        [TestMethod]
//        public void MoveDown_Trigger_Unneeded() => TestTriggerForSingleOperation(m => m.MoveDown(m.Features[1]), false);

//        [TestMethod]
//        public void MoveDown_Trigger_Needed() => TestTriggerForSingleOperation(m => m.MoveDown(m.Features[0]), true);

//        [TestMethod]
//        public void MoveUp_Trigger_Unneeded() => TestTriggerForSingleOperation(m => m.MoveUp(m.Features[0]), false);

//        [TestMethod]
//        public void MoveUp_Trigger_Needed() => TestTriggerForSingleOperation(m => m.MoveUp(m.Features[1]), true);

//        [TestMethod]
//        public void Delete_Trigger()
//        {
//            var manager = Create();
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.Delete(manager.Features[0]);
//            _mocks.VMBinder.Verify(m => m.ModelChange_FeaturesChange(), Times.Exactly(2));
//        }

//        [TestMethod]
//        public void Dispose_DisposesAllFeatures()
//        {
//            var manager = Create();

//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Switcher);
//            manager.Dispose();

//            _mocks.Features[0].Verify(m => m.Dispose());
//            _mocks.Features[1].Verify(m => m.Dispose());
//        }

//        // TODO: Add a sanity check to this function that verifies something *did* change
//        void TestTriggerForSingleOperation(Action<FeatureManager> op, bool needed)
//        {
//            var manager = Create();

//            manager.CreateFeature(FeatureTypes.Unsupported);
//            manager.CreateFeature(FeatureTypes.Unsupported);
//            _mocks.VMBinder.Reset();

//            op(manager);

//            _mocks.VMBinder.Verify(m => m.ModelChange_FeaturesChange(), needed ? Times.Once : Times.Never);
//        }
//    }
//}
