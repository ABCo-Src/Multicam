using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
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
            Mock<ISwitcherInteractionBuffer>[] Buffers,
            Mock<IBinderForSwitcherFeature> UIBinder,
            SwitcherSpecs[] BufferSpecs,
            Mock<ISwitcher> NewISwitcher,
            Mock<IDummySwitcher> NewIDummySwitcher);

        Action<RetrospectiveFadeInfo?>[] _bufferCallbacks = Array.Empty<Action<RetrospectiveFadeInfo?>>();
        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _mocks.InitialDummy = new();
            _mocks.UIBinder = new();
            _mocks.NewISwitcher = new();
            _mocks.NewIDummySwitcher = new();

            _bufferCallbacks = new Action<RetrospectiveFadeInfo?>[] { i => { }, i => { } };
            _mocks.BufferSpecs = new SwitcherSpecs[] { new(), new() };
            _mocks.Buffers = new Mock<ISwitcherInteractionBuffer>[] { CreateBufferMock(0), CreateBufferMock(1) };

            int currentBufferAccessIdx = 0;
            _mocks.Factory = new();
            _mocks.Factory.Setup(m => m.CreateSync(It.IsAny<ISwitcher>())).Returns(() => _mocks.Buffers[currentBufferAccessIdx++].Object);
            _mocks.Factory.Setup(m => m.CreateAsync(It.IsAny<ISwitcher>())).ReturnsAsync(() => _mocks.Buffers[currentBufferAccessIdx++].Object);

            Mock<ISwitcherInteractionBuffer> CreateBufferMock(int idx)
            {
                var newMock = new Mock<ISwitcherInteractionBuffer>();
                newMock.SetupGet(m => m.Specs).Returns(_mocks.BufferSpecs[idx]);
                newMock.SetupGet(m => m.IsConnected).Returns(true);
                newMock.Setup(m => m.SetOnBusChangeFinishCall(It.IsAny<Action<RetrospectiveFadeInfo?>>())).Callback<Action<RetrospectiveFadeInfo?>>(a => _bufferCallbacks[idx] = a);
                return newMock;
            }
        }

        public SwitcherRunningFeature Create() => new(_mocks.InitialDummy.Object, _mocks.Factory.Object, _mocks.UIBinder.Object);

        [TestMethod]
        public void Ctor_CreatesSyncBuffer()
        {
            Create();
            _mocks.Factory.Verify(m => m.CreateSync(_mocks.InitialDummy.Object), Times.Once);
        }

        [TestMethod]
        public void Ctor_FinishesBinderConstruction()
        {
            var feature = Create();
            _mocks.UIBinder.Verify(m => m.FinishConstruction(feature));
        }

        [TestMethod]
        public void FeatureType() => Assert.AreEqual(FeatureTypes.Switcher, Create().FeatureType);

        [TestMethod]
        public void UIBinder() => Assert.AreEqual(_mocks.UIBinder.Object, Create().UIBinder);

        [TestMethod]
        public void GetValue()
        {
            Create().GetValue(3, 8);
            _mocks.Buffers[0].Verify(m => m.GetValue(3, 8), Times.Once);
        }

        [TestMethod]
        public async Task GetValue_PostChange()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
            feature.GetValue(3, 7);
            _mocks.Buffers[1].Verify(m => m.GetValue(3, 7), Times.Once);
        }

        [TestMethod]
        public void SetValue_Dummy()
        {
            var feature = Create();
            feature.PostValue(3, 7, 34);
            _mocks.Buffers[0].Verify(m => m.PostValue(3, 7, 34), Times.Once);
        }

        [TestMethod]
        public async Task SetValue_PostChange()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);

            feature.PostValue(3, 7, 34);

            _mocks.Buffers[1].Verify(m => m.PostValue(3, 7, 34), Times.Once);
        }

        [TestMethod]
        public void SwitcherSpecs()
        {
            var feature = Create();
            Assert.AreEqual(_mocks.BufferSpecs[0], feature.SwitcherSpecs);
            _mocks.Buffers[0].VerifyGet(m => m.Specs, Times.Once);
        }

        [TestMethod]
        public void IsConnected()
        {
            var feature = Create();
            Assert.IsTrue(feature.IsConnected);
            _mocks.Buffers[0].VerifyGet(m => m.IsConnected, Times.Once);
        }

        [TestMethod]
        public async Task ChangeSwitcher_GetsNewBuffer()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
            _mocks.Factory.Verify(m => m.CreateAsync(_mocks.NewISwitcher.Object), Times.Once);
        }

        [TestMethod]
        public async Task ChangeSwitcher_DisposesOld()
        {
            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
            _mocks.Buffers[0].Verify(m => m.Dispose(), Times.Once);
        }

        [TestMethod]
        public async Task ChangeSwitcher_OldNotDisposedDuringAwaits()
        {
            _mocks.Factory.Setup(m => m.CreateAsync(It.IsAny<ISwitcher>())).ReturnsAsync(() =>
            {
                _mocks.Buffers[0].Verify(m => m.Dispose(), Times.Never);
                return _mocks.Buffers[1].Object;
            });

            var feature = Create();
            await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task OnBusChange_TriggersVM(bool postChange)
        {
            var feature = Create();
            if (postChange) await feature.ChangeSwitcherAsync(_mocks.NewISwitcher.Object);
            _bufferCallbacks[postChange ? 1 : 0].Invoke(new RetrospectiveFadeInfo());
            _mocks.UIBinder.Verify(m => m.ModelChange_Specs());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void Cut(int mixBlock)
        {
            Create().Cut(mixBlock);
            _mocks.Buffers[0].Verify(m => m.Cut(mixBlock));
        }

        // TODO: Add test to verify that it also works when switcher is changed to dummy again?
        [TestMethod]
        public void Dispose()
        {
            Create().Dispose();
            _mocks.Buffers[0].Verify(m => m.Dispose(), Times.Once);
        }
    }
}
