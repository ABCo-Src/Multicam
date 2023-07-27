using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Strips.Switchers.Types;
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
        public SwitcherRunningStrip CreateDefault() => new SwitcherRunningStrip(Mock.Of<IDummySwitcher>());
        public SwitcherRunningStrip CreateWithCustomSwitcher(IDummySwitcher switcher) => new SwitcherRunningStrip(switcher);

        [TestMethod]
        public void SwitcherSpecs_Matches()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == mockSpecs);
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);
        }

        [TestMethod]
        public void SwitcherSpecs_NoChangeBetweenGets()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(mockSpecs);

            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);
            Assert.AreEqual(mockSpecs, switcherRunningStrip.SwitcherSpecs);

            // Verify it was only received once
            switcherMock.Verify(m => m.ReceiveSpecs(), Times.Once);
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public void GetValue_Program(SwitcherMixBlockType type)
        {
            var mixBlock = new SwitcherMixBlock(type, new SwitcherBusInput[2] { new(), new() }, type == SwitcherMixBlockType.CutBus ? null : Array.Empty<SwitcherBusInput>());
            TestGetValueNative(mixBlock, 0);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void GetValue_NativePreview(int mixBlockNo)
        {
            var inputs = new SwitcherBusInput[2] { new(), new() };
            var mixBlock = new SwitcherMixBlock(SwitcherMixBlockType.ProgramPreview, inputs, inputs);
            TestGetValueNative(mixBlock, 1);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void GetValue_EmulatedPreview_WithInputs(int mixBlockNo)
        {
            var mixBlock = new SwitcherMixBlock(SwitcherMixBlockType.CutBus, new SwitcherBusInput[2] { new(76, ""), new(35, "") }, null);
            Mock<IDummySwitcher> switcherMock = GetMockWithTwoMixBlocks(mixBlock);
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

            Assert.AreEqual(76, switcherRunningStrip.GetValue(mixBlockNo, 1));
            Assert.AreEqual(76, switcherRunningStrip.GetValue(mixBlockNo, 1));

            // Verify it did NOT try to receive it from the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(mixBlockNo, 1), Times.Never);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void GetValue_EmulatedPreview_NoInputs(int mixBlockNo)
        {
            var mixBlock = new SwitcherMixBlock(SwitcherMixBlockType.CutBus, Array.Empty<SwitcherBusInput>(), null);
            var switcherMock = GetMockWithTwoMixBlocks(mixBlock);
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

            Assert.AreEqual(0, switcherRunningStrip.GetValue(mixBlockNo, 1));

            // Verify it did NOT try to receive it from the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(mixBlockNo, 1), Times.Never);
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public void SetValue_Program(SwitcherMixBlockType type)
        {
            var mixBlock = new SwitcherMixBlock(type, new SwitcherBusInput[] { new(4, ""), new(13, "") }, type == SwitcherMixBlockType.CutBus ? null : Array.Empty<SwitcherBusInput>());
            var switcherMock = GetMockWithTwoMixBlocks(mixBlock);
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

            switcherRunningStrip.SetValueBackground(0, 0, 13);
            switcherRunningStrip.SetValueBackground(1, 0, 4);
                
            switcherMock.Verify(m => m.SendValueAsync(0, 0, 13), Times.Once);
            switcherMock.Verify(m => m.SendValueAsync(1, 0, 4), Times.Once);
        }

        [TestMethod]
        public void SetValue_NativePreview()
        {
            var mixBlock = new SwitcherMixBlock(SwitcherMixBlockType.ProgramPreview, Array.Empty<SwitcherBusInput>(), new SwitcherBusInput[] { new(4, ""), new(13, "") });
            var switcherMock = GetMockWithTwoMixBlocks(mixBlock);
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

            switcherRunningStrip.SetValueBackground(0, 1, 13);
            switcherRunningStrip.SetValueBackground(1, 1, 13);

            switcherMock.Verify(m => m.SendValueAsync(0, 1, 13), Times.Once);
            switcherMock.Verify(m => m.SendValueAsync(1, 1, 13), Times.Once);
        }

        [TestMethod]
        public void SetValue_EmulatedPreview()
        {
            var mixBlock = new SwitcherMixBlock(SwitcherMixBlockType.CutBus, new SwitcherBusInput[] { new(4, ""), new(13, ""), new(28, "") }, null);
            var switcherMock = GetMockWithTwoMixBlocks(mixBlock);
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

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

        void TestGetValueNative(SwitcherMixBlock mockSpecs, int bus)
        {
            var switcherMock = GetMockWithTwoMixBlocks(mockSpecs);
            switcherMock.Setup(m => m.ReceiveValueAsync(0, bus)).Returns(Task.FromResult(2));
            switcherMock.Setup(m => m.ReceiveValueAsync(1, bus)).Returns(Task.FromResult(4));
            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);

            Assert.AreEqual(2, switcherRunningStrip.GetValue(0, bus));
            Assert.AreEqual(2, switcherRunningStrip.GetValue(0, bus));
            Assert.AreEqual(4, switcherRunningStrip.GetValue(1, bus));
            Assert.AreEqual(4, switcherRunningStrip.GetValue(1, bus));

            // Verify it was received once
            switcherMock.Verify(m => m.ReceiveValueAsync(0, bus), Times.Once);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, bus), Times.Once);
        }

        static Mock<IDummySwitcher> GetMockWithTwoMixBlocks(SwitcherMixBlock mixBlock)
        {
            var mockSpecs = new SwitcherSpecs(new[] { mixBlock, mixBlock });
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(mockSpecs);
            return switcherMock;
        }

        [TestMethod]
        public void Dispose_DisposesSwitcher()
        {
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(new SwitcherSpecs());

            var switcherRunningStrip = CreateWithCustomSwitcher(switcherMock.Object);
            switcherRunningStrip.Dispose();

            switcherMock.Verify(m => m.Dispose());
        }
    }
}
