using ABCo.Multicam.Core.Strips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Strips
{
    [TestClass]
    public class UnsupportedRunningStripTests
    {
        [TestMethod]
        public void Dispose_DoesNotThrow() => new UnsupportedRunningStrip().Dispose();
    }
}
