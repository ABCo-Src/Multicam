using ABCo.Multicam.Core;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features.Switcher
{
    [TestClass]
    public class SwitcherActButtonViewModelTests
    {
        public record struct Mocks(Mock<IServiceSource> ServiceSource, Mock<ISwitcherMixBlockVM> Parent);
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.ServiceSource = new Mock<IServiceSource>();
            _mocks.Parent = new Mock<ISwitcherMixBlockVM>();
        }

        public ISwitcherActButtonViewModel Create(bool isAuto)
        {
            ISwitcherActButtonViewModel btn = isAuto ? CreateAuto() : CreateCut();
            btn.FinishConstruction(_mocks.Parent.Object);
            return btn;

            SwitcherCutButtonViewModel CreateCut() => new();
            SwitcherAutoButtonViewModel CreateAuto() => new();
        }

        [TestMethod]
        [DataRow(false, "Cut")]
        [DataRow(true, "Auto")]
        public void Ctor(bool isAuto, string expectedText)
        {
            var vm = Create(isAuto);
            Assert.AreEqual(expectedText, vm.Text);
        }

        [TestMethod]
        public void Click_Cut()
        {
            Create(false).Click();
            _mocks.Parent.Verify(m => m.CutButtonPress(), Times.Once);
        }
    }
}
