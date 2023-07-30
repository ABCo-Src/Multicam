using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.ViewModels.Features.Switcher
{
    [TestClass]
    public class SwitcherBusInputViewModelTests
    {
        public record struct Mocks(Mock<ISwitcherMixBlockVM> Parent, Mock<IServiceSource> ServiceSource);

        SwitcherBusInput _model = new SwitcherBusInput(1, "Cam1");
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.Parent = new Mock<ISwitcherMixBlockVM>();
            _mocks.ServiceSource = new Mock<IServiceSource>();
        }

        public SwitcherBusInputViewModel Create(bool isProgram) => isProgram ? CreateProgram() : CreatePreview();
        public SwitcherProgramInputViewModel CreateProgram() => new(new(_model, _mocks.Parent.Object), _mocks.ServiceSource.Object);
        public SwitcherPreviewInputViewModel CreatePreview() => new(new(_model, _mocks.Parent.Object), _mocks.ServiceSource.Object);

        [TestMethod]
        public void Ctor_Program()
        {
            var vm = CreateProgram();

            Assert.AreEqual(_mocks.Parent.Object, vm.Parent);
            Assert.IsTrue(vm.IsProgram);
            Assert.AreEqual(_model, vm.Base);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        public void Ctor_Preview()
        {
            var vm = CreatePreview();

            Assert.AreEqual(_mocks.Parent.Object, vm.Parent);
            Assert.IsFalse(vm.IsProgram);
            Assert.AreEqual(_model, vm.Base);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Text(bool program)
        {
            var vm = Create(program);
            Assert.AreEqual("Cam1", vm.Text);
        }

        [TestMethod]
        public void Preview_Highlight_True()
        {
            var vm = CreatePreview();
            vm.SetHighlight(false);
            vm.SetHighlight(true);
            Assert.AreEqual(SwitcherButtonStatus.PreviewActive, vm.Status);
        }

        [TestMethod]
        public void Preview_Highlight_False()
        {
            var vm = CreatePreview();
            vm.SetHighlight(true);
            vm.SetHighlight(false);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        public void Program_Highlight_True()
        {
            var vm = CreateProgram();
            vm.SetHighlight(false);
            vm.SetHighlight(true);
            Assert.AreEqual(SwitcherButtonStatus.ProgramActive, vm.Status);
        }

        [TestMethod]
        public void Program_Highlight_False()
        {
            var vm = CreateProgram();
            vm.SetHighlight(true);
            vm.SetHighlight(false);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }
    }
}
