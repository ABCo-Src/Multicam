﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features.Switcher
{
	[TestClass]
    public class SwitcherBusInputVMTests
    {
        public record struct Mocks(Mock<ISwitcherMixBlockVM> Parent, Mock<IServiceSource> ServiceSource);

        readonly SwitcherBusInput _model = new SwitcherBusInput(1, "Cam1");
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.Parent = new Mock<ISwitcherMixBlockVM>();
            _mocks.ServiceSource = new Mock<IServiceSource>();
        }

        public SwitcherBusInputVM Create(bool isProgram)
        {
            SwitcherBusInputVM input = isProgram ? CreateProgram() : CreatePreview();
            input.FinishConstruction(_model, _mocks.Parent.Object);
            return input;

            SwitcherProgramInputVM CreateProgram() => new();
            SwitcherPreviewInputVM CreatePreview() => new();
        }

        [TestMethod]
        public void Ctor_Program()
        {
            var vm = Create(true);

            Assert.AreEqual(_model, vm.Base);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        public void Ctor_Preview()
        {
            var vm = Create(false);

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
            var vm = Create(false);
            vm.SetHighlight(true);
            Assert.AreEqual(SwitcherButtonStatus.PreviewActive, vm.Status);
        }

        [TestMethod]
        public void Preview_Highlight_False()
        {
            var vm = Create(false);
            vm.SetHighlight(false);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        public void Program_Highlight_True()
        {
            var vm = Create(true);
            vm.SetHighlight(true);
            Assert.AreEqual(SwitcherButtonStatus.ProgramActive, vm.Status);
        }

        [TestMethod]
        public void Program_Highlight_False()
        {
            var vm = Create(true);
            vm.SetHighlight(false);
            Assert.AreEqual(SwitcherButtonStatus.NeutralInactive, vm.Status);
        }

        [TestMethod]
        public void Program_Click()
        {
            Create(true).Click();
            _mocks.Parent.Verify(m => m.SetProgram(1), Times.Once);
        }

        [TestMethod]
        public void Preview_Click()
        {
            Create(false).Click();
            _mocks.Parent.Verify(m => m.SetPreview(1), Times.Once);
        }
    }
}
