using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.ViewModels.Features.Switcher
{
    [TestClass]
    public class SwitcherMixBlockViewModelTests
    {
        private static SwitcherMixBlockViewModel CreateDefault(SwitcherMixBlock model) => new SwitcherMixBlockViewModel(model, Mock.Of<IServiceSource>(), Mock.Of<ISwitcherFeatureViewModel>());
        private static SwitcherMixBlockViewModel CreateWithParent(SwitcherMixBlock model, ISwitcherFeatureViewModel parent) => new SwitcherMixBlockViewModel(model, Mock.Of<IServiceSource>(), parent);

        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => 
            new SwitcherMixBlockViewModel(new SwitcherMixBlock(), null!, Mock.Of<ISwitcherFeatureViewModel>()));

        [TestMethod]
        public void Ctor_General()
        {
            var model = new SwitcherMixBlock();
            var parent = Mock.Of<ISwitcherFeatureViewModel>();
            SwitcherMixBlockViewModel vm = CreateWithParent(model, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(model, vm.BaseBlock);
            Assert.IsNotNull(vm.ProgramBus);
            Assert.IsNotNull(vm.PreviewBus);

            Assert.AreEqual(SwitcherActButtonViewModel.Type.Cut, vm.CutButton.Action);
            Assert.AreEqual(vm, vm.CutButton.Parent);
            Assert.AreEqual(SwitcherActButtonViewModel.Type.Auto, vm.AutoButton.Action);
            Assert.AreEqual(vm, vm.AutoButton.Parent);
        }

        [TestMethod]
        public void Ctor_ProgramOnly()
        {
            var busInput1 = new SwitcherBusInput(1, "Cam1");
            var busInput2 = new SwitcherBusInput(2, "Cam2");
            var model = SwitcherMixBlock.NewCutBus(busInput1, busInput2);

            var parent = Mock.Of<ISwitcherFeatureViewModel>();
            SwitcherMixBlockViewModel vm = CreateWithParent(model, parent);

            Assert.AreEqual(2, vm.ProgramBus.Count);
            Assert.AreEqual(busInput1, vm.ProgramBus[0].Base);
            Assert.AreEqual(busInput2, vm.ProgramBus[1].Base);
            Assert.AreEqual(vm, vm.ProgramBus[0].Parent);
            Assert.AreEqual(vm, vm.ProgramBus[1].Parent);
            Assert.IsTrue(vm.ProgramBus[0].IsProgram);
            Assert.IsTrue(vm.ProgramBus[1].IsProgram);
        }

        [TestMethod]
        public void Ctor_NoPreview()
        {
            var model = SwitcherMixBlock.NewCutBus();
            var parent = Mock.Of<ISwitcherFeatureViewModel>();
            SwitcherMixBlockViewModel vm = CreateWithParent(model, parent);

            Assert.AreEqual(0, vm.PreviewBus.Count);
        }

        [TestMethod]
        public void Ctor_Preview()
        {
            var busInput1 = new SwitcherBusInput(1, "Cam1");
            var busInput2 = new SwitcherBusInput(2, "Cam2");
            var model = SwitcherMixBlock.NewProgPrev(Array.Empty<SwitcherBusInput>(), new SwitcherBusInput[2] { busInput1, busInput2 });
            var parent = Mock.Of<ISwitcherFeatureViewModel>();
            SwitcherMixBlockViewModel vm = CreateWithParent(model, parent);

            Assert.AreEqual(2, vm.PreviewBus.Count);
            Assert.AreEqual(busInput1, vm.PreviewBus[0].Base);
            Assert.AreEqual(busInput2, vm.PreviewBus[1].Base);
            Assert.AreEqual(vm, vm.PreviewBus[0].Parent);
            Assert.AreEqual(vm, vm.PreviewBus[1].Parent);
            Assert.IsFalse(vm.PreviewBus[0].IsProgram);
            Assert.IsFalse(vm.PreviewBus[1].IsProgram);
        }

        [TestMethod]
        public void MainLabel_ProgramPreview()
        {
            SwitcherMixBlockViewModel vm = CreateDefault(SwitcherMixBlock.NewProgPrev());
            Assert.AreEqual("Program", vm.MainLabel);
        }

        [TestMethod]
        public void MainLabel_CutBus()
        {
            SwitcherMixBlockViewModel vm = CreateDefault(SwitcherMixBlock.NewCutBus());
            Assert.AreEqual("Cut Bus", vm.MainLabel);
        }

        [TestMethod]
        public void ShowPreview_ProgramPreview()
        {
            SwitcherMixBlockViewModel vm = CreateDefault(SwitcherMixBlock.NewProgPrev());
            Assert.IsTrue(vm.ShowPreview);
        }

        [TestMethod]
        public void ShowPreview_CutBus()
        {
            SwitcherMixBlockViewModel vm = CreateDefault(SwitcherMixBlock.NewCutBus());
            Assert.IsFalse(vm.ShowPreview);
        }
    }
}