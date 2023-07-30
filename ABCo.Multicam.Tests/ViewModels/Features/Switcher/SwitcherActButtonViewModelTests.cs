using ABCo.Multicam.Core;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.ViewModels.Features.Switcher
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

        public SwitcherActButtonViewModel Create(bool isAuto) => isAuto ? CreateAuto() : CreateCut();
        public SwitcherCutButtonViewModel CreateCut() => new(new(null, _mocks.Parent.Object), _mocks.ServiceSource.Object);
        public SwitcherAutoButtonViewModel CreateAuto() => new(new(null, _mocks.Parent.Object), _mocks.ServiceSource.Object);

        [TestMethod]
        [DataRow(false, "Cut")]
        [DataRow(true, "Auto")]
        public void Ctor(bool isAuto, string expectedText)
        {
            var vm = Create(isAuto);

            Assert.AreEqual(expectedText, vm.Text);
            Assert.AreEqual(_mocks.Parent.Object, vm.Parent);
        }
    }
}
