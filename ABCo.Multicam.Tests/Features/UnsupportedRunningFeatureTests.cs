using ABCo.Multicam.Core.Features;
using Moq;
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
        Mock<IBinderForUnsupportedFeature> _binderMock = null!;

        [TestInitialize]
        public void SetupMocks()
        {
            _binderMock = new();
        }

        public UnsupportedRunningFeature Create() => new(_binderMock.Object);

        [TestMethod]
        public void UIBinder() => Assert.AreEqual(_binderMock.Object, Create().UIBinder);

        [TestMethod]
        public void FeatureType() => Assert.AreEqual(FeatureTypes.Unsupported, Create().FeatureType);

        [TestMethod]
        public void Dispose_DoesNotThrow() => Create().Dispose();
    }
}
