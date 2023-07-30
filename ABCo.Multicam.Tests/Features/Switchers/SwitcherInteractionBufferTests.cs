using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers
{
    [TestClass]
    public class SwitcherInteractionBufferTests
    {
        public record struct Mocks(Mock<ISwitcher> Switcher);
        Action<SwitcherBusChangeInfo> _switcherBusChangeCallback = v => { };
        SwitcherSpecs _switcherSpecs = new();
        Mocks _mocks = new();

        [TestInitialize]
        public void MakeMocks()
        {
            _switcherSpecs = new();
            _mocks.Switcher = Mock.Get(Mock.Of<ISwitcher>());
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(true);
            _mocks.Switcher.Setup(m => m.ReceiveSpecs()).Returns(() => _switcherSpecs);
            _mocks.Switcher.Setup(m => m.SetOnBusChangeFinishCall(It.IsAny<Action<SwitcherBusChangeInfo>>())).Callback<Action<SwitcherBusChangeInfo>>(v => _switcherBusChangeCallback = v);
            _mocks.Switcher.Setup(m => m.ReceiveValue(0, 0)).Returns(2);
            _mocks.Switcher.Setup(m => m.ReceiveValue(0, 1)).Returns(4);
            _mocks.Switcher.Setup(m => m.ReceiveValue(1, 0)).Returns(6);
            _mocks.Switcher.Setup(m => m.ReceiveValue(1, 1)).Returns(8);
        }

        public async Task<SwitcherInteractionBuffer> Create() => await SwitcherInteractionBuffer.CreateAsync(_mocks.Switcher.Object);

        [TestMethod]
        public async Task Ctor_Disconnected()
        {
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);
            var feature = await Create();
            Assert.AreEqual(0, feature.Specs.MixBlocks.Count);
            Assert.IsFalse(feature.IsConnected);
        }

        [TestMethod]
        public async Task Ctor_Disconnected_NoAccess()
        {
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);
            var feature = await Create();
            _mocks.Switcher.Verify(m => m.ReceiveSpecs(), Times.Never);
            _mocks.Switcher.Verify(m => m.PostValue(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task Ctor_Connected()
        {
            var feature = await Create();
            Assert.IsTrue(feature.IsConnected);
        }

        [TestMethod]
        public async Task IsConnected()
        {
            var feature = await Create();
            Assert.IsTrue(feature.IsConnected);
            Assert.IsTrue(feature.IsConnected);

            // Verify it was only received once
            _mocks.Switcher.VerifyGet(m => m.IsConnected, Times.Once);
        }

        [TestMethod]
        public async Task SwitcherSpecs()
        {
            var feature = await Create();
            Assert.AreEqual(_switcherSpecs, feature.Specs);
            Assert.AreEqual(_switcherSpecs, feature.Specs);

            // Verify it was only received once
            _mocks.Switcher.Verify(m => m.ReceiveSpecs(), Times.Once);
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public async Task GetValue_Program(SwitcherMixBlockType type)
        {
            var inputs = new SwitcherBusInput[2] { new(), new() };
            var mixBlock = type == SwitcherMixBlockType.CutBus ? SwitcherMixBlock.NewCutBus(inputs) : SwitcherMixBlock.NewProgPrev(inputs);
            await TestChangeAndGetValueNative(mixBlock, 0);
        }

        [TestMethod]
        public async Task GetValue_NativePreview()
        {
            var mixBlock = SwitcherMixBlock.NewProgPrev(Array.Empty<SwitcherBusInput>(), new(), new());
            await TestChangeAndGetValueNative(mixBlock, 1);
        }

        [TestMethod]
        public async Task GetValue_EmulatedPreview_WithInputs()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus(new(76, ""), new(35, ""));
            _switcherSpecs = new(mixBlock, mixBlock);

            var feature = await Create();
            Assert.AreEqual(76, feature.GetValue(0, 1));
            Assert.AreEqual(76, feature.GetValue(0, 1));
            Assert.AreEqual(76, feature.GetValue(1, 1));
            Assert.AreEqual(76, feature.GetValue(1, 1));

            // Verify NO access to the switcher
            _mocks.Switcher.Verify(m => m.ReceiveValue(0, 1), Times.Never);
            _mocks.Switcher.Verify(m => m.ReceiveValue(1, 1), Times.Never);
        }

        [TestMethod]
        public async Task GetValue_EmulatedPreview_NoInputs()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus();
            _switcherSpecs = new(mixBlock, mixBlock);

            var feature = await Create();
            Assert.AreEqual(0, feature.GetValue(0, 1));
            Assert.AreEqual(0, feature.GetValue(0, 1));
            Assert.AreEqual(0, feature.GetValue(1, 1));
            Assert.AreEqual(0, feature.GetValue(1, 1));

            // Verify NO access to the switcher
            _mocks.Switcher.Verify(m => m.ReceiveValue(0, 1), Times.Never);
            _mocks.Switcher.Verify(m => m.ReceiveValue(1, 1), Times.Never);
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public async Task SetValue_Program(SwitcherMixBlockType type)
        {
            var inputs = new SwitcherBusInput[2] { new(4, ""), new(13, "") };
            var mixBlock = type == SwitcherMixBlockType.CutBus ? SwitcherMixBlock.NewCutBus(inputs) : SwitcherMixBlock.NewProgPrev(inputs);
            _switcherSpecs = new(mixBlock, mixBlock);

            var feature = await Create();
            feature.PostValue(0, 0, 13);
            feature.PostValue(1, 0, 4);

            _mocks.Switcher.Verify(m => m.PostValue(0, 0, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.PostValue(1, 0, 4), Times.Once);
        }

        [TestMethod]
        public async Task SetValue_NativePreview()
        {
            var mixBlock = SwitcherMixBlock.NewProgPrev(Array.Empty<SwitcherBusInput>(), new(4, ""), new(13, ""));
            _switcherSpecs = new(mixBlock, mixBlock);

            var feature = await Create();
            feature.PostValue(0, 1, 13);
            feature.PostValue(1, 1, 4);

            _mocks.Switcher.Verify(m => m.PostValue(0, 1, 13), Times.Once);
            _mocks.Switcher.Verify(m => m.PostValue(1, 1, 4), Times.Once);
        }

        [TestMethod]
        public async Task SetValue_EmulatedPreview()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus(new(4, ""), new(13, ""), new(28, ""));
            _switcherSpecs = new(mixBlock, mixBlock);

            var feature = await Create();
            feature.PostValue(0, 1, 13);
            Assert.AreEqual(13, feature.GetValue(0, 1));
            feature.PostValue(1, 1, 28);
            Assert.AreEqual(28, feature.GetValue(1, 1));

            // Verify NO access to the switcher
            _mocks.Switcher.Verify(m => m.ReceiveValue(0, 1), Times.Never);
            _mocks.Switcher.Verify(m => m.ReceiveValue(1, 1), Times.Never);
            _mocks.Switcher.Verify(m => m.PostValue(0, 1, 13), Times.Never);
            _mocks.Switcher.Verify(m => m.PostValue(1, 1, 28), Times.Never);
        }

        [TestMethod]
        public async Task SetValue_Disconnected()
        {
            _switcherSpecs = new(SwitcherMixBlock.NewProgPrevSameInputs(new SwitcherBusInput(4, "")));
            _mocks.Switcher.Setup(m => m.IsConnected).Returns(false);

            var feature = await Create();
            feature.PostValue(0, 0, 13);

            _mocks.Switcher.Verify(m => m.PostValue(0, 0, 13), Times.Never);
        }

        [TestMethod]
        public async Task Dispose()
        {
            var feature = await Create();
            feature.Dispose();
            _mocks.Switcher.Verify(m => m.Dispose(), Times.Once);
        }

        [TestMethod]
        public async Task OnBusChange_KnownProgram_Updates()
        {
            _switcherSpecs = new(SwitcherMixBlock.NewCutBus(new(1, ""), new(2, "")), SwitcherMixBlock.NewProgPrevSameInputs(new(1, ""), new(8, "")));
            var feature = await Create();
            _switcherBusChangeCallback(new SwitcherBusChangeInfo(true, 1, 0, 13, null));

            Assert.AreEqual(2, feature.GetValue(0, 0));
            Assert.AreEqual(1, feature.GetValue(0, 1));
            Assert.AreEqual(13, feature.GetValue(1, 0));
            Assert.AreEqual(8, feature.GetValue(1, 1));
        }

        [TestMethod]
        public async Task OnBusChange_KnownPreview_Updates()
        {
            _switcherSpecs = new(SwitcherMixBlock.NewCutBus(new(1, ""), new(2, "")), SwitcherMixBlock.NewProgPrevSameInputs(new(8, ""), new(13, "")));
            var feature = await Create();
            _switcherBusChangeCallback(new SwitcherBusChangeInfo(true, 1, 1, 13, null));

            Assert.AreEqual(2, feature.GetValue(0, 0));
            Assert.AreEqual(1, feature.GetValue(0, 1));
            Assert.AreEqual(6, feature.GetValue(1, 0));
            Assert.AreEqual(13, feature.GetValue(1, 1));
        }

        [TestMethod]
        public async Task OnBusChange_UnknownProgram_Updates()
        {
            _switcherSpecs = new(SwitcherMixBlock.NewCutBus(new(1, ""), new(2, "")), SwitcherMixBlock.NewProgPrevSameInputs(new(1, ""), new(8, "")));
            var feature = await Create();
            _mocks.Switcher.Setup(m => m.ReceiveValue(0, 0)).Returns(1);

            _switcherBusChangeCallback(new SwitcherBusChangeInfo(false, 0, 0, 0, null));

            Assert.AreEqual(1, feature.GetValue(0, 0));
            Assert.AreEqual(1, feature.GetValue(0, 1));
            Assert.AreEqual(6, feature.GetValue(1, 0));
            Assert.AreEqual(8, feature.GetValue(1, 1));
        }

        [TestMethod]
        public async Task OnBusChange_UnknownPreview_Updates()
        {
            _switcherSpecs = new(SwitcherMixBlock.NewCutBus(new(1, ""), new(2, "")), SwitcherMixBlock.NewProgPrevSameInputs(new(1, ""), new(8, "")));
            var feature = await Create();
            _mocks.Switcher.Setup(m => m.ReceiveValue(1, 1)).Returns(1);

            _switcherBusChangeCallback(new SwitcherBusChangeInfo(false, 0, 0, 0, null));

            Assert.AreEqual(2, feature.GetValue(0, 0));
            Assert.AreEqual(1, feature.GetValue(0, 1));
            Assert.AreEqual(6, feature.GetValue(1, 0));
            Assert.AreEqual(1, feature.GetValue(1, 1));
        }

        [TestMethod]
        [DataRow(false, 0, 0)]
        [DataRow(false, 1, 1)]
        [DataRow(true, 0, 0)]
        [DataRow(true, 1, 1)]
        public async Task OnBusChange_TriggersCallback(bool isKnown, int mixBlock, int bus)
        {
            _switcherSpecs = new(SwitcherMixBlock.NewCutBus(new(1, ""), new(2, "")), SwitcherMixBlock.NewProgPrevSameInputs(new(1, ""), new(8, "")));
            var feature = await Create();

            bool ran = false;
            feature.SetOnBusChangeFinishCall(i => 
            {
                Assert.AreEqual(new RetrospectiveFadeInfo(), i);
                ran = true;
            });

            _switcherBusChangeCallback(new SwitcherBusChangeInfo(isKnown, mixBlock, bus, 0, new RetrospectiveFadeInfo()));

            Assert.IsTrue(ran);
        }

        async Task TestChangeAndGetValueNative(SwitcherMixBlock mixBlock, int bus)
        {
            _switcherSpecs = new(mixBlock, mixBlock);
            var feature = await Create();

            Assert.AreEqual(bus == 0 ? 2 : 4, feature.GetValue(0, bus));
            Assert.AreEqual(bus == 0 ? 2 : 4, feature.GetValue(0, bus));
            Assert.AreEqual(bus == 0 ? 6 : 8, feature.GetValue(1, bus));
            Assert.AreEqual(bus == 0 ? 6 : 8, feature.GetValue(1, bus));

            // Verify it was received once
            _mocks.Switcher.Verify(m => m.ReceiveValue(0, bus), Times.Once);
            _mocks.Switcher.Verify(m => m.ReceiveValue(1, bus), Times.Once);
        }
    }
}
