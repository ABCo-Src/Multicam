using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Structures;
using ABCo.Multicam.UI.ViewModels.Strips;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Strips
{
    [TestClass]
    public class StripManagerTests
    {
        static StripManager CreateDefault()
        {
            var mock = new Mock<IServiceSource>();
            mock.Setup(m => m.Get<IUnsupportedRunningStrip>()).Returns(() => Mock.Of<IUnsupportedRunningStrip>());
            return new StripManager(mock.Object);
        }

        static StripManager CreateWithCustomSource(IServiceSource src) => new StripManager(src);

        [TestMethod]
        public void Ctor()
        {
            var manager = CreateDefault();
            Assert.IsNotNull(manager.Strips);
        }

        [TestMethod]
        public void CreateStrip_AddsStrip()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            Assert.AreEqual(1, manager.Strips.Count);
        }

        [TestMethod]
        public void CreateStrip_Switcher() => TestAddStripType<ISwitcherRunningStrip>(StripTypes.Switcher);

        [TestMethod]
        public void CreateStrip_Unsupported() => TestAddStripType<IUnsupportedRunningStrip>(StripTypes.Tally);

        private static void TestAddStripType<T>(StripTypes type) where T : class
        {
            var mock = new Mock<IServiceSource>();
            mock.Setup(m => m.Get<T>()).Returns(Mock.Of<T>());

            var manager = CreateWithCustomSource(mock.Object);
            manager.CreateStrip(type);

            mock.Verify(m => m.Get<T>(), Times.Once);
            Assert.IsTrue(manager.Strips[0].GetType().IsAssignableTo(typeof(T)));
        }

        [TestMethod]
        public void MoveDown_OneItem()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            var newStrip = manager.Strips[0];

            manager.MoveDown(newStrip);

            Assert.AreEqual(newStrip, manager.Strips[0]);
        }

        [TestMethod]
        public void MoveDown_OnTop()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            var movingStrip = manager.Strips[0];
            var unmovingStrip = manager.Strips[1];

            manager.MoveDown(movingStrip);

            Assert.AreEqual(unmovingStrip, manager.Strips[0]);
            Assert.AreEqual(movingStrip, manager.Strips[1]);
        }

        [TestMethod]
        public void MoveDown_OnBottom()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            var movingStrip = manager.Strips[1];
            var unmovingStrip = manager.Strips[0];

            manager.MoveDown(movingStrip);

            Assert.AreEqual(unmovingStrip, manager.Strips[0]);
            Assert.AreEqual(movingStrip, manager.Strips[1]);
        }

        [TestMethod]
        public void MoveDown_Middle()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            var unmoving1 = manager.Strips[0];
            var movingStrip = manager.Strips[1];
            var unmoving2 = manager.Strips[2];

            manager.MoveDown(movingStrip);

            Assert.AreEqual(unmoving1, manager.Strips[0]);
            Assert.AreEqual(unmoving2, manager.Strips[1]);
            Assert.AreEqual(movingStrip, manager.Strips[2]);
        }

        [TestMethod]
        public void MoveUp_OneItem()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            var newStrip = manager.Strips[0];

            manager.MoveUp(newStrip);

            Assert.AreEqual(newStrip, manager.Strips[0]);
        }

        [TestMethod]
        public void MoveUp_OnTop()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            var movingStrip = manager.Strips[0];
            var unmovingStrip = manager.Strips[1];

            manager.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, manager.Strips[0]);
            Assert.AreEqual(unmovingStrip, manager.Strips[1]);
        }

        [TestMethod]
        public void MoveUp_OnBottom()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            var movingStrip = manager.Strips[1];
            var unmovingStrip = manager.Strips[0];

            manager.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, manager.Strips[0]);
            Assert.AreEqual(unmovingStrip, manager.Strips[1]);
        }

        [TestMethod]
        public void MoveUp_Middle()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            var unmoving1 = manager.Strips[0];
            var movingStrip = manager.Strips[1];
            var unmoving2 = manager.Strips[2];

            manager.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, manager.Strips[0]);
            Assert.AreEqual(unmoving1, manager.Strips[1]);
            Assert.AreEqual(unmoving2, manager.Strips[2]);
        }

        [TestMethod]
        public void Delete()
        {
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            var unmoving1 = manager.Strips[0];
            var movingStrip = manager.Strips[1];
            var unmoving2 = manager.Strips[2];

            manager.Delete(movingStrip);

            Assert.AreEqual(unmoving1, manager.Strips[0]);
            Assert.AreEqual(unmoving2, manager.Strips[1]);
        }

        [TestMethod]
        public void Delete_DisposesStrip()
        {
            var mockStrip = new Mock<IUnsupportedRunningStrip>();
            var serviceSource = Mock.Of<IServiceSource>(s => s.Get<IUnsupportedRunningStrip>() == mockStrip.Object);
            var manager = CreateWithCustomSource(serviceSource);

            manager.CreateStrip(StripTypes.Unsupported);
            manager.Delete(mockStrip.Object);

            mockStrip.Verify(m => m.Dispose());
        }

        [TestMethod]
        public void CreateStrip_Trigger()
        {
            bool triggered = false;
            var manager = CreateDefault();
            manager.SetStripsChangeForVM(() => triggered = true);

            manager.CreateStrip(StripTypes.Unsupported);
            Assert.IsTrue(triggered);
        }

        [TestMethod]
        public void MoveDown_Trigger_Unneeded() => TestTriggerForSingleOperation(m => m.MoveDown(m.Strips[1]), false);

        [TestMethod]
        public void MoveDown_Trigger_Needed() => TestTriggerForSingleOperation(m => m.MoveDown(m.Strips[0]), true);

        [TestMethod]
        public void MoveUp_Trigger_Unneeded() => TestTriggerForSingleOperation(m => m.MoveUp(m.Strips[0]), false);

        [TestMethod]
        public void MoveUp_Trigger_Needed() => TestTriggerForSingleOperation(m => m.MoveUp(m.Strips[1]), true);

        [TestMethod]
        public void Delete_Trigger()
        {
            bool triggered = false;
            var manager = CreateDefault();
            manager.CreateStrip(StripTypes.Unsupported);

            manager.SetStripsChangeForVM(() => triggered = true);

            manager.Delete(manager.Strips[0]);
            Assert.IsTrue(triggered);
        }

        [TestMethod]
        public void Dispose_DisposesAllStrips()
        {
            var mockStrip1 = new Mock<IUnsupportedRunningStrip>();
            var mockStrip2 = new Mock<ISwitcherRunningStrip>();
            var serviceSource = Mock.Of<IServiceSource>(s => s.Get<IUnsupportedRunningStrip>() == mockStrip1.Object && s.Get<ISwitcherRunningStrip>() == mockStrip2.Object);
            var manager = CreateWithCustomSource(serviceSource);

            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Switcher);
            manager.Dispose();

            mockStrip1.Verify(m => m.Dispose());
            mockStrip2.Verify(m => m.Dispose());
        }

        // TODO: Add a sanity check to this function that verifies something *did* change
        static void TestTriggerForSingleOperation(Action<StripManager> op, bool needed)
        {
            bool triggered = false;
            var manager = CreateDefault();

            manager.CreateStrip(StripTypes.Unsupported);
            manager.CreateStrip(StripTypes.Unsupported);
            manager.SetStripsChangeForVM(() => triggered = true);

            op(manager);

            Assert.AreEqual(needed, triggered);
        }
    }
}
