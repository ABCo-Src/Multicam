using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Structures;
using ABCo.Multicam.Core.Switchers;
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
            var project = CreateDefault();
            Assert.IsNotNull(project.Strips);
        }

        [TestMethod]
        public void CreateStrip_AddsStrip()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            Assert.AreEqual(1, project.Strips.Count);
        }

        [TestMethod]
        public void CreateStrip_Switcher() => TestAddStripType<ISwitcherRunningStrip>(StripTypes.Switcher);

        [TestMethod]
        public void CreateStrip_Unsupported() => TestAddStripType<IUnsupportedRunningStrip>(StripTypes.Tally);

        private static void TestAddStripType<T>(StripTypes type) where T : class
        {
            var mock = new Mock<IServiceSource>();
            mock.Setup(m => m.Get<T>()).Returns(Mock.Of<T>());

            var project = CreateWithCustomSource(mock.Object);
            project.CreateStrip(type);

            mock.Verify(m => m.Get<T>(), Times.Once);
            Assert.IsTrue(project.Strips[0].GetType().IsAssignableTo(typeof(T)));
        }

        [TestMethod]
        public void MoveDown_OneItem()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            var newStrip = project.Strips[0];

            project.MoveDown(newStrip);

            Assert.AreEqual(newStrip, project.Strips[0]);
        }

        [TestMethod]
        public void MoveDown_OnTop()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            var movingStrip = project.Strips[0];
            var unmovingStrip = project.Strips[1];

            project.MoveDown(movingStrip);

            Assert.AreEqual(unmovingStrip, project.Strips[0]);
            Assert.AreEqual(movingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveDown_OnBottom()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            var movingStrip = project.Strips[1];
            var unmovingStrip = project.Strips[0];

            project.MoveDown(movingStrip);

            Assert.AreEqual(unmovingStrip, project.Strips[0]);
            Assert.AreEqual(movingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveDown_Middle()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            var unmoving1 = project.Strips[0];
            var movingStrip = project.Strips[1];
            var unmoving2 = project.Strips[2];

            project.MoveDown(movingStrip);

            Assert.AreEqual(unmoving1, project.Strips[0]);
            Assert.AreEqual(unmoving2, project.Strips[1]);
            Assert.AreEqual(movingStrip, project.Strips[2]);
        }

        [TestMethod]
        public void MoveUp_OneItem()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            var newStrip = project.Strips[0];

            project.MoveUp(newStrip);

            Assert.AreEqual(newStrip, project.Strips[0]);
        }

        [TestMethod]
        public void MoveUp_OnTop()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            var movingStrip = project.Strips[0];
            var unmovingStrip = project.Strips[1];

            project.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, project.Strips[0]);
            Assert.AreEqual(unmovingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveUp_OnBottom()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            var movingStrip = project.Strips[1];
            var unmovingStrip = project.Strips[0];

            project.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, project.Strips[0]);
            Assert.AreEqual(unmovingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveUp_Middle()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            var unmoving1 = project.Strips[0];
            var movingStrip = project.Strips[1];
            var unmoving2 = project.Strips[2];

            project.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, project.Strips[0]);
            Assert.AreEqual(unmoving1, project.Strips[1]);
            Assert.AreEqual(unmoving2, project.Strips[2]);
        }

        [TestMethod]
        public void Delete()
        {
            var project = CreateDefault();
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            project.CreateStrip(StripTypes.Unsupported);
            var unmoving1 = project.Strips[0];
            var movingStrip = project.Strips[1];
            var unmoving2 = project.Strips[2];

            project.Delete(movingStrip);

            Assert.AreEqual(unmoving1, project.Strips[0]);
            Assert.AreEqual(unmoving2, project.Strips[1]);
        }

        [TestMethod]
        public void CreateStrip_Trigger()
        {
            bool triggered = false;
            var project = CreateDefault();
            project.SetStripsChangeForVM(() => triggered = true);

            project.CreateStrip(StripTypes.Unsupported);
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
