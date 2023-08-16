using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers.Interaction
{
    [TestClass]
    public class SwitcherInteractionBufferTests
    {
        public record struct Mocks(
            Mock<ISwitcher> Switcher,
            Mock<ISwitcherInteractionBufferFactory> Factory,
            Mock<IMixBlockInteractionBuffer>[] Buffers,
            Action<RetrospectiveFadeInfo>?[] CacheChangeCallbacks);

        Action<SwitcherBusChangeInfo> _switcherBusChangeCallback = v => { };
        SwitcherSpecs _switcherSpecs = new();
        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _switcherSpecs = new(
                SwitcherMixBlock.NewProgPrevSameInputs(new(), new SwitcherBusInput(2, ""), new SwitcherBusInput(4, "")),
                SwitcherMixBlock.NewProgPrevSameInputs(new(), new SwitcherBusInput(6, ""), new SwitcherBusInput(8, ""))
            );

            _mocks.Switcher = new();
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(true);
            _mocks.Switcher.Setup(m => m.ReceiveSpecs()).Returns(() => _switcherSpecs);
            _mocks.Switcher.Setup(m => m.SetOnBusChangeFinishCall(It.IsAny<Action<SwitcherBusChangeInfo>>())).Callback<Action<SwitcherBusChangeInfo>>(v => _switcherBusChangeCallback = v);

            _mocks.CacheChangeCallbacks = new Action<RetrospectiveFadeInfo>?[2];
            _mocks.Buffers = new Mock<IMixBlockInteractionBuffer>[] { new(), new() };
            _mocks.Buffers[0].Setup(m => m.SetCacheChangeExceptRefreshCall(It.IsAny<Action<RetrospectiveFadeInfo>>())).Callback<Action<RetrospectiveFadeInfo>>(a => _mocks.CacheChangeCallbacks[0] = a);
            _mocks.Buffers[1].Setup(m => m.SetCacheChangeExceptRefreshCall(It.IsAny<Action<RetrospectiveFadeInfo>>())).Callback<Action<RetrospectiveFadeInfo>>(a => _mocks.CacheChangeCallbacks[1] = a);

            _mocks.Factory = new();
            _mocks.Factory.Setup(m => m.CreateMixBlock(It.IsAny<SwitcherMixBlock>(), 0, It.IsAny<ISwitcher>())).Returns(_mocks.Buffers[0].Object);
            _mocks.Factory.Setup(m => m.CreateMixBlock(It.IsAny<SwitcherMixBlock>(), 1, It.IsAny<ISwitcher>())).Returns(_mocks.Buffers[1].Object);
        }

        public SwitcherInteractionBuffer Create() => new SwitcherInteractionBuffer(_mocks.Switcher.Object, _mocks.Factory.Object);

        [TestMethod]
        public void Ctor_Disconnected()
        {
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);

            var feature = Create();
            Assert.AreEqual(0, feature.Specs.MixBlocks.Count);
            Assert.IsFalse(feature.IsConnected);

            _mocks.Switcher.Verify(m => m.ReceiveSpecs(), Times.Never);
            _mocks.Factory.Verify(m => m.CreateMixBlock(It.IsAny<SwitcherMixBlock>(), It.IsAny<int>(), It.IsAny<ISwitcher>()), Times.Never);
        }

        [TestMethod]
        public void Ctor_Connected()
        {
            Assert.IsTrue(Create().IsConnected);

            _mocks.Switcher.Verify(m => m.ReceiveSpecs(), Times.Once);
            _mocks.Factory.Verify(m => m.CreateMixBlock(_switcherSpecs.MixBlocks[0], 0, _mocks.Switcher.Object));
            _mocks.Factory.Verify(m => m.CreateMixBlock(_switcherSpecs.MixBlocks[1], 1, _mocks.Switcher.Object));
        }

        [TestMethod]
        public void IsConnected()
        {
            var feature = Create();
            Assert.IsTrue(feature.IsConnected);
            Assert.IsTrue(feature.IsConnected);

            // Verify it was only received once
            _mocks.Switcher.VerifyGet(m => m.IsConnected, Times.Once);
        }

        [TestMethod]
        public void SwitcherSpecs()
        {
            var feature = Create();
            Assert.AreEqual(_switcherSpecs, feature.Specs);
            Assert.AreEqual(_switcherSpecs, feature.Specs);

            // Verify it was only received once
            _mocks.Switcher.Verify(m => m.ReceiveSpecs(), Times.Once);
        }

        void Verify1Success1Fail(int mixBlock, Action<Mock<IMixBlockInteractionBuffer>, Times> per)
        {
            per(_mocks.Buffers[mixBlock], Times.Once());
            per(_mocks.Buffers[(~mixBlock & 1)], Times.Never());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void GetValue_Program(int mixBlock)
        {
            Create().GetValue(mixBlock, 0);
            Verify1Success1Fail(mixBlock, (i, t) => i.VerifyGet(m => m.Program, t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void GetValue_Preview(int mixBlock)
        {
            Create().GetValue(mixBlock, 1);
            Verify1Success1Fail(mixBlock, (i, t) => i.VerifyGet(m => m.Preview, t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SetBusValue_Program(int mixBlock)
        {
            Create().PostValue(mixBlock, 0, 198);
            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.SetProgram(198), t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SetBusValue_Preview(int mixBlock)
        {
            Create().PostValue(mixBlock, 1, 124);
            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.SetPreview(124), t));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void SetValue_Disconnected(int mixBlock)
        {
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);

            var feature = Create();
            feature.PostValue(mixBlock, 0, 13);
            feature.PostValue(mixBlock, 1, 19);

            _mocks.Buffers[0].Verify(m => m.SetProgram(13), Times.Never);
            _mocks.Buffers[0].Verify(m => m.SetPreview(19), Times.Never);
            _mocks.Buffers[1].Verify(m => m.SetProgram(13), Times.Never);
            _mocks.Buffers[1].Verify(m => m.SetPreview(19), Times.Never);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void OnBusChange_KnownProgram(int mixBlock)
        {
            var feature = Create();
            _switcherBusChangeCallback(new SwitcherBusChangeInfo(true, mixBlock, 0, 13, null));

            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.RefreshWithKnownProg(13), t));

            for (int i = 0; i < 1; i++)
            {
                _mocks.Buffers[i].Verify(m => m.RefreshWithKnownPrev(13), Times.Never);
                _mocks.Buffers[i].Verify(m => m.RefreshCache(), Times.Never);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void OnBusChange_KnownPreview(int mixBlock)
        {
            var feature = Create();
            _switcherBusChangeCallback(new SwitcherBusChangeInfo(true, mixBlock, 1, 13, null));

            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.RefreshWithKnownPrev(13), t));

            for (int i = 0; i < 1; i++)
            {
                _mocks.Buffers[i].Verify(m => m.RefreshWithKnownProg(13), Times.Never);
                _mocks.Buffers[i].Verify(m => m.RefreshCache(), Times.Never);
            }
        }

        [TestMethod]
        public void OnBusChange_Unknown()
        {
            var feature = Create();
            _switcherBusChangeCallback(new SwitcherBusChangeInfo(false, 0, 0, 0, null));

            _mocks.Buffers[0].Verify(m => m.RefreshCache(), Times.Once);
            _mocks.Buffers[1].Verify(m => m.RefreshCache(), Times.Once);
        }

        [TestMethod]
        [DataRow(false, 0, 0)]
        [DataRow(false, 1, 1)]
        [DataRow(true, 0, 0)]
        [DataRow(true, 1, 1)]
        public void OnBusChange_TriggersCallback(bool isKnown, int mixBlock, int bus)
        {
            var feature = Create();

            bool ran = false;
            feature.SetOnBusChangeFinishCall(i =>
            {
                Assert.AreEqual(new RetrospectiveFadeInfo(), i);
                ran = true;
            });

            _switcherBusChangeCallback(new SwitcherBusChangeInfo(isKnown, mixBlock, bus, 0, new RetrospectiveFadeInfo()));

            Assert.IsTrue(ran);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void OnCacheChange_TriggersCallback(int mixBlock)
        {
            var feature = Create();

            bool ran = false;
            feature.SetOnBusChangeFinishCall(i =>
            {
                Assert.AreEqual(new RetrospectiveFadeInfo(), i);
                ran = true;
            });

            _mocks.CacheChangeCallbacks[mixBlock]?.Invoke(new RetrospectiveFadeInfo());

            Assert.IsTrue(ran);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void Cut(int mixBlock)
        {
            Create().Cut(mixBlock);
            Verify1Success1Fail(mixBlock, (i, t) => i.Verify(m => m.Cut(), t));
        }

        [TestMethod]
        public void Dispose()
        {
            var feature = Create();
            feature.Dispose();
            _mocks.Switcher.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
