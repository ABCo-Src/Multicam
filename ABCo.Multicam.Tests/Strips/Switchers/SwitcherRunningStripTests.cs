using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using ABCo.Multicam.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Strips.Switchers
{
    [TestClass]
    public class SwitcherRunningStripTests
    {
        public SwitcherRunningStrip CreateDefault() => new(Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == new SwitcherSpecs()));
        public SwitcherRunningStrip CreateWithSpecs(SwitcherSpecs specs) => new(Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == specs));
        public SwitcherRunningStrip CreateWithSwitcher(IDummySwitcher switcher) => new(switcher);

        [TestMethod]
        public void SwitcherSpecs_Matches()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == mockSpecs);
            var switcherRunningStrip = CreateWithSwitcher(switcherMock);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);
        }

        [TestMethod]
        public void SwitcherSpecs_NoChangeBetweenGets()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(mockSpecs);

            var switcherRunningStrip = CreateWithSwitcher(switcherMock.Object);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);

            // Verify it was only received once
            switcherMock.Verify(m => m.ReceiveSpecs(), Times.Once);
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public async Task GetValue_Program(SwitcherMixBlockType type)
        {
            var inputs = new SwitcherBusInput[2] { new(), new() };
            var mixBlock = type == SwitcherMixBlockType.CutBus ? SwitcherMixBlock.NewCutBus(inputs) : SwitcherMixBlock.NewProgPrev(inputs);

            await TestGetValueNative(mixBlock, 0);
        }

        [TestMethod]
        public async Task GetValue_InitialDummy()
        {
            var switcher = CreateWithSpecs(new(SwitcherMixBlock.NewProgPrevSameInputs(new(), new()), SwitcherMixBlock.NewProgPrevSameInputs(new(), new())));
            Assert.AreEqual(1, switcher.GetValue(0, 0));
            Assert.AreEqual(1, switcher.GetValue(0, 1));
            Assert.AreEqual(1, switcher.GetValue(1, 0));
            Assert.AreEqual(1, switcher.GetValue(1, 1));
        }

        [TestMethod]
        public async Task GetValue_NativePreview()
        {
            var mixBlock = SwitcherMixBlock.NewProgPrev(Array.Empty<SwitcherBusInput>(), new(), new());
            await TestGetValueNative(mixBlock, 1);
        }

        [TestMethod]
        public async Task GetValue_EmulatedPreview_WithInputs()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus(new(76, ""), new(35, ""));
            var switcherRunningStrip = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningStrip, mixBlock);

            Assert.AreEqual(76, switcherRunningStrip.GetValue(0, 1));
            Assert.AreEqual(76, switcherRunningStrip.GetValue(0, 1));
            Assert.AreEqual(76, switcherRunningStrip.GetValue(1, 1));
            Assert.AreEqual(76, switcherRunningStrip.GetValue(1, 1));

            // Verify NO access to the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(0, 1), Times.Never);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, 1), Times.Never);
        }

        [TestMethod]
        public async Task GetValue_EmulatedPreview_NoInputs()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus();
            var switcherRunningStrip = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningStrip, mixBlock);

            Assert.AreEqual(0, switcherRunningStrip.GetValue(0, 1));
            Assert.AreEqual(0, switcherRunningStrip.GetValue(0, 1));
            Assert.AreEqual(0, switcherRunningStrip.GetValue(1, 1));
            Assert.AreEqual(0, switcherRunningStrip.GetValue(1, 1));

            // Verify NO access to the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(0, 1), Times.Never);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, 1), Times.Never);
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public async Task SetValue_Program(SwitcherMixBlockType type)
        {
            var inputs = new SwitcherBusInput[2] { new(4, ""), new(13, "") };
            var mixBlock = type == SwitcherMixBlockType.CutBus ? SwitcherMixBlock.NewCutBus(inputs) : SwitcherMixBlock.NewProgPrev(inputs);
            var switcherRunningStrip = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningStrip, mixBlock);

            switcherRunningStrip.SetValueBackground(0, 0, 13);
            switcherRunningStrip.SetValueBackground(1, 0, 4);
                
            switcherMock.Verify(m => m.SendValueAsync(0, 0, 13), Times.Once);
            switcherMock.Verify(m => m.SendValueAsync(1, 0, 4), Times.Once);
        }

        [TestMethod]
        public async Task SetValue_NativePreview()
        {
            var mixBlock = SwitcherMixBlock.NewProgPrev(Array.Empty<SwitcherBusInput>(), new(4, ""), new(13, ""));
            var switcherRunningStrip = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningStrip, mixBlock);

            switcherRunningStrip.SetValueBackground(0, 1, 13);
            switcherRunningStrip.SetValueBackground(1, 1, 4);

            switcherMock.Verify(m => m.SendValueAsync(0, 1, 13), Times.Once);
            switcherMock.Verify(m => m.SendValueAsync(1, 1, 4), Times.Once);
        }

        [TestMethod]
        public async Task SetValue_EmulatedPreview()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus(new(4, ""), new(13, ""), new(28, ""));
            var switcherRunningStrip = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningStrip, mixBlock);

            switcherRunningStrip.SetValueBackground(0, 1, 13);
            Assert.AreEqual(13, switcherRunningStrip.GetValue(0, 1));
            switcherRunningStrip.SetValueBackground(1, 1, 28);
            Assert.AreEqual(28, switcherRunningStrip.GetValue(1, 1));

            // Verify NO access to the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(0, 1), Times.Never);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, 1), Times.Never);
            switcherMock.Verify(m => m.SendValueAsync(0, 1, 13), Times.Never);
            switcherMock.Verify(m => m.SendValueAsync(1, 1, 28), Times.Never);
        }

        async Task TestGetValueNative(SwitcherMixBlock mixBlock, int bus)
        {
            var switcherMock = new Mock<ISwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecsAsync()).ReturnsAsync(new SwitcherSpecs(mixBlock, mixBlock));
            switcherMock.Setup(m => m.ReceiveValueAsync(0, bus)).ReturnsAsync(2);
            switcherMock.Setup(m => m.ReceiveValueAsync(1, bus)).ReturnsAsync(4);

            var switcherRunningStrip = CreateDefault();
            await switcherRunningStrip.ChangeSwitcherAsync(switcherMock.Object);

            Assert.AreEqual(2, switcherRunningStrip.GetValue(0, bus));
            Assert.AreEqual(2, switcherRunningStrip.GetValue(0, bus));
            Assert.AreEqual(4, switcherRunningStrip.GetValue(1, bus));
            Assert.AreEqual(4, switcherRunningStrip.GetValue(1, bus));

            // Verify it was received once
            switcherMock.Verify(m => m.ReceiveValueAsync(0, bus), Times.Once);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, bus), Times.Once);
        }

        static async Task<Mock<ISwitcher>> ChangeSwitcherToMockWithTwoMBs(SwitcherRunningStrip strip, SwitcherMixBlock mixBlock)
        {
            var switcherMock = new Mock<ISwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecsAsync()).ReturnsAsync(new SwitcherSpecs(mixBlock, mixBlock));
            await strip.ChangeSwitcherAsync(switcherMock.Object);

            return switcherMock;
        }

        [TestMethod]
        public async Task ChangeSwitcher_ChangesSpecs()
        {
            var strip = CreateDefault();

            var switcher2Specs = new SwitcherSpecs();
            var switcher2 = Mock.Of<ISwitcher>(m => m.ReceiveSpecsAsync() == Task.FromResult(switcher2Specs));
            await strip.ChangeSwitcherAsync(switcher2);

            Assert.AreEqual(switcher2Specs, strip.SwitcherSpecs);
        }

        [TestMethod]
        public async Task ChangeSwitcher_DisposesOld()
        {
            var switcher1 = Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == new SwitcherSpecs());
            var strip = CreateWithSwitcher(switcher1);

            await strip.ChangeSwitcherAsync(Mock.Of<ISwitcher>(m => m.ReceiveSpecsAsync() == Task.FromResult(new SwitcherSpecs())));

            Mock.Get(switcher1).Verify(m => m.Dispose(), Times.Once);
        }

        // Verifies ChangeSwitcher doesn't leave the class in a "half-changed" state while it's awaiting, by verifying nothing changes until the end:
        [TestMethod]
        public async Task ChangeSwitcher_NoDataTearing()
        {
            var strip = CreateWithSpecs(new(SwitcherMixBlock.NewCutBus()));

            var switcher = new Mock<ISwitcher>();
            var switcher2Specs = new SwitcherSpecs(SwitcherMixBlock.NewProgPrevSameInputs(new(), new(), new(), new()));

            switcher.Setup(m => m.ReceiveSpecsAsync()).ReturnsAsync(() =>
            {
                AssertNoChanges();
                return switcher2Specs;
            });

            switcher.Setup(m => m.ReceiveValueAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(() =>
            {
                AssertNoChanges();
                return 4;
            });

            void AssertNoChanges()
            {
                if (strip.SwitcherSpecs == switcher2Specs || strip.GetValue(0, 0) == 4 || strip.GetRawSwitcher() == switcher.Object)
                    Assert.Fail("Aspects changed before completion");
            }

            await strip.ChangeSwitcherAsync(switcher.Object);
        }

        [TestMethod]
        public void Dispose_DisposesSwitcher()
        {
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(new SwitcherSpecs());

            var switcherRunningStrip = CreateWithSwitcher(switcherMock.Object);
            switcherRunningStrip.Dispose();

            switcherMock.Verify(m => m.Dispose());
        }
    }
}
