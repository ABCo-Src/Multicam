using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers
{
    [TestClass]
    public class SwitcherRunningFeatureTests
    {
        public record struct Mocks(Mock<IDummySwitcher> InitialDummy, 
            Mock<ISwitcherInteractionBufferFactory> Factory, 
            Mock<ISwitcherInteractionBuffer> FactoryDummyBuffer, 
            SwitcherSpecs FactoryDummyBufferSpecs,
            Mock<ISwitcherInteractionBuffer> FactoryRealBuffer,
            SwitcherSpecs FactoryRealBufferSpecs,
            Mock<ISwitcher> NewISwitcher,
            Mock<IDummySwitcher> NewIDummySwitcher);

        Action<RetrospectiveFadeInfo?> _factoryDummyBufferCallback = i => { };
        Action<RetrospectiveFadeInfo?> _factoryRealBufferCallback = i => { };
        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _mocks.InitialDummy = new Mock<IDummySwitcher>();
            _mocks.NewISwitcher = new Mock<ISwitcher>();
            _mocks.NewIDummySwitcher = new Mock<IDummySwitcher>();

            _mocks.FactoryDummyBufferSpecs = new SwitcherSpecs();
            _mocks.FactoryDummyBuffer = new Mock<ISwitcherInteractionBuffer>();
            _mocks.FactoryDummyBuffer.SetupGet(m => m.Specs).Returns(_mocks.FactoryDummyBufferSpecs);
            _mocks.FactoryDummyBuffer.SetupGet(m => m.IsConnected).Returns(true);
            _mocks.FactoryDummyBuffer.Setup(m => m.SetOnBusChangeFinishCall(It.IsAny<Action<RetrospectiveFadeInfo?>>())).Callback<Action<RetrospectiveFadeInfo?>>(a => _factoryDummyBufferCallback = a);

            _mocks.FactoryRealBufferSpecs = new SwitcherSpecs();
            _mocks.FactoryRealBuffer = new Mock<ISwitcherInteractionBuffer>();
            _mocks.FactoryRealBuffer.SetupGet(m => m.Specs).Returns(_mocks.FactoryRealBufferSpecs);
            _mocks.FactoryRealBuffer.Setup(m => m.SetOnBusChangeFinishCall(It.IsAny<Action<RetrospectiveFadeInfo?>>())).Callback<Action<RetrospectiveFadeInfo?>>(a => _factoryRealBufferCallback = a);

            _mocks.Factory = New<ISwitcherInteractionBufferFactory>(m =>
                m.CreateDummy(It.IsAny<IDummySwitcher>()) == _mocks.FactoryDummyBuffer.Object && 
                m.CreateRealAsync(It.IsAny<ISwitcher>()) == Task.FromResult(_mocks.FactoryRealBuffer.Object));

            Mock<T> New<T>(Expression<Func<T, bool>> expr) where T : class => Mock.Get(Mock.Of(expr));
        }

        public SwitcherRunningFeature Create() => new(_mocks.InitialDummy.Object, _mocks.Factory.Object);

        [TestMethod]
        public void Ctor_NewDummyBuffer()
        {
            Create();
            _mocks.Factory.Verify(m => m.CreateDummy(_mocks.InitialDummy.Object), Times.Once);
        }

        [TestMethod]
        public void GetValue_Dummy()
        {
            Create().GetValue(3, 8);
            _mocks.FactoryDummyBuffer.Verify(m => m.GetValue(3, 8), Times.Once);
        }

        [TestMethod]
        public async Task GetValue_PostChange()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
            feature.GetValue(3, 7);
            _mocks.FactoryRealBuffer.Verify(m => m.GetValue(3, 7), Times.Once);
        }

        [TestMethod]
        public void SetValue_Dummy()
        {
            var feature = Create();
            feature.PostValue(3, 7, 34);
            _mocks.FactoryDummyBuffer.Verify(m => m.PostValue(3, 7, 34), Times.Once);
        }

        [TestMethod]
        public async Task SetValue_PostChange()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);

            feature.PostValue(3, 7, 34);

            _mocks.FactoryRealBuffer.Verify(m => m.PostValue(3, 7, 34), Times.Once);
        }

        [TestMethod]
        public void SwitcherSpecs()
        {
            var feature = Create();
            Assert.AreEqual(_mocks.FactoryDummyBufferSpecs, feature.SwitcherSpecs);
            _mocks.FactoryDummyBuffer.VerifyGet(m => m.Specs, Times.Once);
        }

        [TestMethod]
        public void IsConnected()
        {
            var feature = Create();
            Assert.IsTrue(feature.IsConnected);
            _mocks.FactoryDummyBuffer.VerifyGet(m => m.IsConnected, Times.Once);
        }

        [TestMethod]
        public async Task ChangeSwitcher_Real_GetsRealBuffer()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
            _mocks.Factory.Verify(m => m.CreateRealAsync(_mocks.NewISwitcher.Object), Times.Once);
        }

        [TestMethod]
        public async Task ChangeSwitcher_Dummy_GetsDummyBuffer()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewIDummySwitcher.Object);
            _mocks.Factory.Verify(m => m.CreateDummy(_mocks.NewIDummySwitcher.Object), Times.Once);
        }

        [TestMethod]
        public async Task ChangeSwitcher_DisposesOld()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
            _mocks.FactoryDummyBuffer.Verify(m => m.Dispose(), Times.Once);
        }

        [TestMethod]
        public async Task ChangeSwitcher_OldNotDisposedDuringAwaits()
        {
            _mocks.Factory.Setup(m => m.CreateRealAsync(It.IsAny<ISwitcher>())).ReturnsAsync(() =>
            {
                _mocks.FactoryDummyBuffer.Verify(m => m.Dispose(), Times.Never);
                return _mocks.FactoryRealBuffer.Object;
            });

            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
        }

        [TestMethod]
        public void OnBusChange_Dummy_TriggersVM()
        {
            var feature = Create();

            bool ran = false;
            feature.SetOnBusChangeFinishForVM(i =>
            {
                Assert.AreEqual(new RetrospectiveFadeInfo(), i);
                ran = true;
            });

            _factoryDummyBufferCallback(new RetrospectiveFadeInfo());
            Assert.IsTrue(ran);
        }

        [TestMethod]
        public async Task OnBusChange_Real_TriggersVM()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);

            bool ran = false;
            feature.SetOnBusChangeFinishForVM(i =>
            {
                Assert.AreEqual(new RetrospectiveFadeInfo(), i);
                ran = true;
            });

            _factoryRealBufferCallback(new RetrospectiveFadeInfo());
            Assert.IsTrue(ran);
        }

        // TODO: Add test to verify that it also works when switcher is changed to dummy again?

        [TestMethod]
        public void Dispose()
        {
            Create().Dispose();
            _mocks.FactoryDummyBuffer.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
