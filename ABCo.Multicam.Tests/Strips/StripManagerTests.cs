using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.UI.ViewModels.Strips;
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
        [TestMethod]
        public void Ctor()
        {
            var project = new StripManager();
            Assert.IsNotNull(project.Strips);
        }

        [TestMethod]
        public void CreateStrip_Normal_AddsStrip()
        {
            var project = new StripManager();
            project.CreateStrip();
            Assert.AreEqual(1, project.Strips.Count);
        }

        [TestMethod]
        public void MoveDown_OneItem()
        {
            var project = new StripManager();
            project.CreateStrip();
            var newStrip = project.Strips[0];

            project.MoveDown(newStrip);

            Assert.AreEqual(newStrip, project.Strips[0]);
        }

        [TestMethod]
        public void MoveDown_OnTop()
        {
            var project = new StripManager();
            project.CreateStrip();
            project.CreateStrip();
            var movingStrip = project.Strips[0];
            var unmovingStrip = project.Strips[1];

            project.MoveDown(movingStrip);

            Assert.AreEqual(unmovingStrip, project.Strips[0]);
            Assert.AreEqual(movingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveDown_OnBottom()
        {
            var project = new StripManager();
            project.CreateStrip();
            project.CreateStrip();
            var movingStrip = project.Strips[1];
            var unmovingStrip = project.Strips[0];

            project.MoveDown(movingStrip);

            Assert.AreEqual(unmovingStrip, project.Strips[0]);
            Assert.AreEqual(movingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveDown_Middle()
        {
            var project = new StripManager();
            project.CreateStrip();
            project.CreateStrip();
            project.CreateStrip();
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
            var project = new StripManager();
            project.CreateStrip();
            var newStrip = project.Strips[0];

            project.MoveUp(newStrip);

            Assert.AreEqual(newStrip, project.Strips[0]);
        }

        [TestMethod]
        public void MoveUp_OnTop()
        {
            var project = new StripManager();
            project.CreateStrip();
            project.CreateStrip();
            var movingStrip = project.Strips[0];
            var unmovingStrip = project.Strips[1];

            project.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, project.Strips[0]);
            Assert.AreEqual(unmovingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveUp_OnBottom()
        {
            var project = new StripManager();
            project.CreateStrip();
            project.CreateStrip();
            var movingStrip = project.Strips[1];
            var unmovingStrip = project.Strips[0];

            project.MoveUp(movingStrip);

            Assert.AreEqual(movingStrip, project.Strips[0]);
            Assert.AreEqual(unmovingStrip, project.Strips[1]);
        }

        [TestMethod]
        public void MoveUp_Middle()
        {
            var project = new StripManager();
            project.CreateStrip();
            project.CreateStrip();
            project.CreateStrip();
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
            var project = new StripManager();
            project.CreateStrip();
            project.CreateStrip();
            project.CreateStrip();
            var unmoving1 = project.Strips[0];
            var movingStrip = project.Strips[1];
            var unmoving2 = project.Strips[2];

            project.Delete(movingStrip);

            Assert.AreEqual(unmoving1, project.Strips[0]);
            Assert.AreEqual(unmoving2, project.Strips[1]);
        }
    }
}
