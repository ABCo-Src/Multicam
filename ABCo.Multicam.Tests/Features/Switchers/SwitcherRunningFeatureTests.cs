using ABCo.Multicam.Core.Features.Switchers;
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

        Mocks _mocks = new Mocks();

        [TestInitialize]
        public void MakeMocks()
        {
            _mocks.InitialDummy = new Mock<IDummySwitcher>();
            _mocks.NewISwitcher = new Mock<ISwitcher>();
            _mocks.NewIDummySwitcher = new Mock<IDummySwitcher>();

            _mocks.FactoryDummyBufferSpecs = new SwitcherSpecs();
            _mocks.FactoryDummyBuffer = New<ISwitcherInteractionBuffer>(m => m.Specs == _mocks.FactoryDummyBufferSpecs && m.IsConnected == true);
            _mocks.FactoryRealBufferSpecs = new SwitcherSpecs();
            _mocks.FactoryRealBuffer = New<ISwitcherInteractionBuffer>(m => m.Specs == _mocks.FactoryRealBufferSpecs);

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
        [DataRow(false)]
        [DataRow(true)]
        public async Task SetValue_Dummy(bool useAsync)
        {
            var feature = Create();

            if (useAsync)
                await feature.SetValueAndWaitAsync(3, 7, 34);
            else
                feature.SetValueBackground(3, 7, 34);

            _mocks.FactoryDummyBuffer.Verify(m => m.SetValueAsync(3, 7, 34), Times.Once);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task SetValue_PostChange(bool useAsync)
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);

            if (useAsync)
                await feature.SetValueAndWaitAsync(3, 7, 34);
            else
                feature.SetValueBackground(3, 7, 34);

            _mocks.FactoryRealBuffer.Verify(m => m.SetValueAsync(3, 7, 34), Times.Once);
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
    }
}
