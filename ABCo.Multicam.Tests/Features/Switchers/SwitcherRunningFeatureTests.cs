using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Features.Switchers
{
    [TestClass]
    public class SwitcherRunningFeatureTests
    {
        public SwitcherRunningFeature CreateDefault() => new(Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == new SwitcherSpecs()));
        public SwitcherRunningFeature CreateWithSpecs(SwitcherSpecs specs) => new(Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == specs));
        public SwitcherRunningFeature CreateWithSwitcher(IDummySwitcher switcher) => new(switcher);

        [TestMethod]
        public void SwitcherSpecs_Matches()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == mockSpecs);
            var switcherRunningFeature = CreateWithSwitcher(switcherMock);
            Assert.AreEqual(mockSpecs, switcherRunningFeature.SwitcherSpecs);
        }

        [TestMethod]
        public void SwitcherSpecs_NoChangeBetweenGets()
        {
            var mockSpecs = new SwitcherSpecs(Array.Empty<SwitcherMixBlock>());
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(mockSpecs);

            var switcherRunningFeature = CreateWithSwitcher(switcherMock.Object);
            Assert.AreEqual(mockSpecs, switcherRunningFeature.SwitcherSpecs);
            Assert.AreEqual(mockSpecs, switcherRunningFeature.SwitcherSpecs);

            // Verify it was only received once
            switcherMock.Verify(m => m.ReceiveSpecs(), Times.Once);
        }

        [TestMethod]
        public void GetValue_InitialDummy()
        {
            var switcher = CreateWithSpecs(new(SwitcherMixBlock.NewProgPrevSameInputs(new(), new()), SwitcherMixBlock.NewProgPrevSameInputs(new(), new())));
            Assert.AreEqual(1, switcher.GetValue(0, 0));
            Assert.AreEqual(1, switcher.GetValue(0, 1));
            Assert.AreEqual(1, switcher.GetValue(1, 0));
            Assert.AreEqual(1, switcher.GetValue(1, 1));
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public async Task ChangeAndGet_Program(SwitcherMixBlockType type)
        {
            var inputs = new SwitcherBusInput[2] { new(), new() };
            var mixBlock = type == SwitcherMixBlockType.CutBus ? SwitcherMixBlock.NewCutBus(inputs) : SwitcherMixBlock.NewProgPrev(inputs);

            await TestChangeAndGetValueNative(mixBlock, 0);
        }

        [TestMethod]
        public async Task ChangeAndGet_NativePreview()
        {
            var mixBlock = SwitcherMixBlock.NewProgPrev(Array.Empty<SwitcherBusInput>(), new(), new());
            await TestChangeAndGetValueNative(mixBlock, 1);
        }

        [TestMethod]
        public async Task ChangeAndGet_EmulatedPreview_WithInputs()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus(new(76, ""), new(35, ""));
            var switcherRunningFeature = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningFeature, mixBlock);

            Assert.AreEqual(76, switcherRunningFeature.GetValue(0, 1));
            Assert.AreEqual(76, switcherRunningFeature.GetValue(0, 1));
            Assert.AreEqual(76, switcherRunningFeature.GetValue(1, 1));
            Assert.AreEqual(76, switcherRunningFeature.GetValue(1, 1));

            // Verify NO access to the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(0, 1), Times.Never);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, 1), Times.Never);
        }

        [TestMethod]
        public async Task ChangeAndGet_EmulatedPreview_NoInputs()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus();
            var switcherRunningFeature = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningFeature, mixBlock);

            Assert.AreEqual(0, switcherRunningFeature.GetValue(0, 1));
            Assert.AreEqual(0, switcherRunningFeature.GetValue(0, 1));
            Assert.AreEqual(0, switcherRunningFeature.GetValue(1, 1));
            Assert.AreEqual(0, switcherRunningFeature.GetValue(1, 1));

            // Verify NO access to the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(0, 1), Times.Never);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, 1), Times.Never);
        }

        [TestMethod]
        [DataRow(SwitcherMixBlockType.CutBus)]
        [DataRow(SwitcherMixBlockType.ProgramPreview)]
        public async Task ChangeAndSet_Program(SwitcherMixBlockType type)
        {
            var inputs = new SwitcherBusInput[2] { new(4, ""), new(13, "") };
            var mixBlock = type == SwitcherMixBlockType.CutBus ? SwitcherMixBlock.NewCutBus(inputs) : SwitcherMixBlock.NewProgPrev(inputs);
            var switcherRunningFeature = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningFeature, mixBlock);

            switcherRunningFeature.SetValueBackground(0, 0, 13);
            switcherRunningFeature.SetValueBackground(1, 0, 4);
                
            switcherMock.Verify(m => m.SendValueAsync(0, 0, 13), Times.Once);
            switcherMock.Verify(m => m.SendValueAsync(1, 0, 4), Times.Once);
        }

        [TestMethod]
        public async Task ChangeAndSet_NativePreview()
        {
            var mixBlock = SwitcherMixBlock.NewProgPrev(Array.Empty<SwitcherBusInput>(), new(4, ""), new(13, ""));
            var switcherRunningFeature = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningFeature, mixBlock);

            switcherRunningFeature.SetValueBackground(0, 1, 13);
            switcherRunningFeature.SetValueBackground(1, 1, 4);

            switcherMock.Verify(m => m.SendValueAsync(0, 1, 13), Times.Once);
            switcherMock.Verify(m => m.SendValueAsync(1, 1, 4), Times.Once);
        }

        [TestMethod]
        public async Task ChangeAndSet_EmulatedPreview()
        {
            var mixBlock = SwitcherMixBlock.NewCutBus(new(4, ""), new(13, ""), new(28, ""));
            var switcherRunningFeature = CreateDefault();
            var switcherMock = await ChangeSwitcherToMockWithTwoMBs(switcherRunningFeature, mixBlock);

            switcherRunningFeature.SetValueBackground(0, 1, 13);
            Assert.AreEqual(13, switcherRunningFeature.GetValue(0, 1));
            switcherRunningFeature.SetValueBackground(1, 1, 28);
            Assert.AreEqual(28, switcherRunningFeature.GetValue(1, 1));

            // Verify NO access to the switcher
            switcherMock.Verify(m => m.ReceiveValueAsync(0, 1), Times.Never);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, 1), Times.Never);
            switcherMock.Verify(m => m.SendValueAsync(0, 1, 13), Times.Never);
            switcherMock.Verify(m => m.SendValueAsync(1, 1, 28), Times.Never);
        }

        [TestMethod]
        public async Task ChangeAndSet_Disconnected()
        {
            var mixBlock = SwitcherMixBlock.NewProgPrevSameInputs(new SwitcherBusInput(4, ""));
            var feature = CreateDefault();

            // TODO: Change IsConnected *after* ChangeSwitcher so the specs properly assign.
            var switcherMock = Mock.Of<ISwitcher>(m => m.IsConnected == false);
            await feature.ChangeSwitcherAsync(switcherMock);

            await feature.SetValueAndWaitAsync(0, 0, 13);
            Mock.Get(switcherMock).Verify(m => m.SendValueAsync(0, 0, 13), Times.Never);
        }

        async Task TestChangeAndGetValueNative(SwitcherMixBlock mixBlock, int bus)
        {
            var switcherMock = new Mock<ISwitcher>();
            switcherMock.Setup(m => m.IsConnected).Returns(true);
            switcherMock.Setup(m => m.ReceiveSpecsAsync()).ReturnsAsync(new SwitcherSpecs(mixBlock, mixBlock));
            switcherMock.Setup(m => m.ReceiveValueAsync(0, bus)).ReturnsAsync(2);
            switcherMock.Setup(m => m.ReceiveValueAsync(1, bus)).ReturnsAsync(4);

            var switcherRunningFeature = CreateDefault();
            await switcherRunningFeature.ChangeSwitcherAsync(switcherMock.Object);

            Assert.AreEqual(2, switcherRunningFeature.GetValue(0, bus));
            Assert.AreEqual(2, switcherRunningFeature.GetValue(0, bus));
            Assert.AreEqual(4, switcherRunningFeature.GetValue(1, bus));
            Assert.AreEqual(4, switcherRunningFeature.GetValue(1, bus));

            // Verify it was received once
            switcherMock.Verify(m => m.ReceiveValueAsync(0, bus), Times.Once);
            switcherMock.Verify(m => m.ReceiveValueAsync(1, bus), Times.Once);
        }

        static async Task<Mock<ISwitcher>> ChangeSwitcherToMockWithTwoMBs(SwitcherRunningFeature feature, SwitcherMixBlock mixBlock)
        {
            var switcherMock = new Mock<ISwitcher>();
            switcherMock.Setup(m => m.IsConnected).Returns(true);
            switcherMock.Setup(m => m.ReceiveSpecsAsync()).ReturnsAsync(new SwitcherSpecs(mixBlock, mixBlock));
            await feature.ChangeSwitcherAsync(switcherMock.Object);

            return switcherMock;
        }

        [TestMethod]
        public async Task ChangeSwitcher_Disconnected_BlankSpecs()
        {
            var feature = CreateDefault();            
            await feature.ChangeSwitcherAsync(Mock.Of<ISwitcher>(m => m.IsConnected == false));
            Assert.AreEqual(0, feature.SwitcherSpecs.MixBlocks.Count);
        }

        [TestMethod]
        public async Task ChangeSwitcher_Disconnected_ClearsCacheState()
        {
            var feature = CreateWithSpecs(new(SwitcherMixBlock.NewProgPrevSameInputs(new SwitcherBusInput())));
            await feature.ChangeSwitcherAsync(Mock.Of<ISwitcher>(m => m.IsConnected == false));
            Assert.ThrowsException<IndexOutOfRangeException>(() => feature.GetValue(0, 0));
        }

        [TestMethod]
        public async Task ChangeSwitcher_Disconnected_ClearsIsConnected()
        {
            var feature = CreateWithSpecs(new(SwitcherMixBlock.NewProgPrevSameInputs(new SwitcherBusInput())));
            await feature.ChangeSwitcherAsync(Mock.Of<ISwitcher>(m => m.IsConnected == false));
            Assert.IsFalse(feature.IsConnected);
        }

        [TestMethod]
        public async Task ChangeSwitcher_Connected_ChangesSpecs()
        {
            var feature = CreateDefault();

            var switcher2Specs = new SwitcherSpecs();
            var switcher2 = Mock.Of<ISwitcher>(m => m.ReceiveSpecsAsync() == Task.FromResult(switcher2Specs) && m.IsConnected == true);
            await feature.ChangeSwitcherAsync(switcher2);

            Assert.AreEqual(switcher2Specs, feature.SwitcherSpecs);
        }

        [TestMethod]
        public async Task ChangeSwitcher_Connected_SetsIsConnected()
        {
            var feature = CreateDefault();

            var switcher = Mock.Of<ISwitcher>(m => m.IsConnected == false);
            await feature.ChangeSwitcherAsync(switcher);

            var switcher2 = Mock.Of<ISwitcher>(m => m.ReceiveSpecsAsync() == Task.FromResult(new SwitcherSpecs()) && m.IsConnected == true);
            await feature.ChangeSwitcherAsync(switcher2);

            Assert.IsTrue(feature.IsConnected);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ChangeSwitcher_UpdatesSwitcher(bool connected)
        {
            var feature = CreateWithSpecs(new(SwitcherMixBlock.NewProgPrevSameInputs(new SwitcherBusInput())));
            var switcher = Mock.Of<ISwitcher>(m => m.ReceiveSpecsAsync() == Task.FromResult(new SwitcherSpecs()) && m.IsConnected == connected);
            await feature.ChangeSwitcherAsync(switcher);
            Assert.AreEqual(switcher, feature.GetRawSwitcher());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ChangeSwitcher_DisposesOld(bool connected)
        {
            var switcher1 = Mock.Of<IDummySwitcher>(m => m.ReceiveSpecs() == new SwitcherSpecs());
            var feature = CreateWithSwitcher(switcher1);

            var switcher2 = Mock.Of<ISwitcher>(m => m.ReceiveSpecsAsync() == Task.FromResult(new SwitcherSpecs()) && m.IsConnected == connected);
            await feature.ChangeSwitcherAsync(switcher2);
            Mock.Get(switcher1).Verify(m => m.Dispose(), Times.Once);

            var switcher3 = Mock.Of<ISwitcher>(m => m.ReceiveSpecsAsync() == Task.FromResult(new SwitcherSpecs()) && m.IsConnected == connected);
            await feature.ChangeSwitcherAsync(switcher3);
            Mock.Get(switcher2).Verify(m => m.Dispose(), Times.Once);
        }

        // Verifies ChangeSwitcher doesn't leave the class in a "half-changed" state while it's awaiting, by verifying nothing changes until the end:
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ChangeSwitcher_NoDataTearing(bool connected)
        {
            var feature = CreateWithSpecs(new(SwitcherMixBlock.NewCutBus()));

            var switcher = new Mock<ISwitcher>();
            var switcher2Specs = new SwitcherSpecs(SwitcherMixBlock.NewProgPrevSameInputs(new(), new(), new(), new()));

            switcher.Setup(m => m.IsConnected).Returns(connected);

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
                if (feature.SwitcherSpecs == switcher2Specs || feature.GetValue(0, 0) == 4 || feature.GetRawSwitcher() == switcher.Object)
                    Assert.Fail("Aspects changed before completion");
            }

            await feature.ChangeSwitcherAsync(switcher.Object);
        }

        [TestMethod]
        public void IsConnected_Default()
        {
            var feature = CreateDefault();
            Assert.IsTrue(feature.IsConnected);
        }

        [TestMethod]
        public void Dispose_DisposesSwitcher()
        {
            var switcherMock = new Mock<IDummySwitcher>();
            switcherMock.Setup(m => m.ReceiveSpecs()).Returns(new SwitcherSpecs());

            var switcherRunningFeature = CreateWithSwitcher(switcherMock.Object);
            switcherRunningFeature.Dispose();

            switcherMock.Verify(m => m.Dispose());
        }
    }
}
