using ABCo.Multicam.Core.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features
{
    [TestClass]
    public class UnsupportedRunningFeatureTests
    {
        [TestMethod]
        public void Dispose_DoesNotThrow() => new UnsupportedRunningFeature().Dispose();
    }
}
