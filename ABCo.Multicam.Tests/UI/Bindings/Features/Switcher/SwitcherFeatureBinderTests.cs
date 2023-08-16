using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.Bindings.Features.Switcher
{
    [TestClass]
    public class SwitcherFeatureBinderTests
    {
        public interface IMB : IVMBinder<IVMForSwitcherMixBlock>, IBinderForSwitcherMixBlock { }

        public record struct Mocks(
            Mock<IServiceSource> ServSource, 
            Mock<IMB>[] Binders,
            Mock<ISwitcherRunningFeature> Model, 
            SwitcherSpecs Specs
        );

        Mocks _mocks;

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.Specs = new SwitcherSpecs(
                SwitcherMixBlock.NewProgPrev(new()),
                SwitcherMixBlock.NewCutBus(new()),
                SwitcherMixBlock.NewProgPrev(new())
            );

            _mocks.Binders = new Mock<IMB>[] { new(), new(), new() };

            _mocks.ServSource = new(MockBehavior.Strict);
            _mocks.ServSource.SetupSequence(m => m.Get<IBinderForSwitcherMixBlock>())
                .Returns(_mocks.Binders[0].Object)
                .Returns(_mocks.Binders[1].Object)
                .Returns(_mocks.Binders[2].Object);

            _mocks.Model = new();
            _mocks.Model.SetupGet(m => m.SwitcherSpecs).Returns(new SwitcherSpecs());
        }

        public SwitcherFeatureVMBinder Create()
        {
            var binder = new SwitcherFeatureVMBinder(_mocks.ServSource.Object);
            binder.FinishConstruction(_mocks.Model.Object);
            return binder;
        }

        [TestMethod]
        public void GetMixBlocks()
        {
            var buffer = Create();
            _mocks.Model.SetupGet(m => m.SwitcherSpecs).Returns(_mocks.Specs);

            var mixBlocks = buffer.GetMixBlocks();

            Assert.AreEqual(3, mixBlocks.Length);
            for (int i = 0; i < 3; i++)
            {
                _mocks.Binders[i].Verify(m => m.FinishConstruction(_mocks.Model.Object, _mocks.Specs.MixBlocks[i], i));
                Assert.AreEqual(_mocks.Binders[i].Object, mixBlocks[i]);
            }
        }

        [TestMethod]
        public void ModelChange_Bus()
        {
            var buffer = Create();
            _mocks.Model.SetupGet(m => m.SwitcherSpecs).Returns(_mocks.Specs);

            var mixBlocks = buffer.GetMixBlocks();
            buffer.ModelChange_Bus();

            Assert.AreEqual(3, mixBlocks.Length);
            for (int i = 0; i < 3; i++)
            {
                _mocks.Binders[i].Verify(m => m.FinishConstruction(_mocks.Model.Object, _mocks.Specs.MixBlocks[i], i), Times.Once);
                _mocks.Binders[i].Verify(m => m.ModelChange_Bus());
                Assert.AreEqual(_mocks.Binders[i].Object, mixBlocks[i]);
            }
        }
    }
}