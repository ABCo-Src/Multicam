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
        public record struct Mocks(
            Mock<IServiceSource> ServiceSource, 
            Mock<ISwitcherFeatureVM> Parent,
            Mock<ISwitcherCutButtonViewModel> Cut,
            Mock<ISwitcherAutoButtonViewModel> Auto,
            Mock<ISwitcherProgramInputViewModel>[] ProgInputs,
            Mock<ISwitcherPreviewInputViewModel>[] PrevInputs,
            SwitcherBusInput[] ModelInputs
            );

        int _progInputPos = 0;
        int _prevInputPos = 0;
        SwitcherMixBlock _model = new();
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _progInputPos = 0;
            _prevInputPos = 0;
            _model = new();

            _mocks.Parent = new Mock<ISwitcherFeatureVM>();

            _mocks.ModelInputs = new SwitcherBusInput[] { new(2, "abc"), new(3, "ghi"), new(5, "def"), new(4, "kl") };
            _mocks.ProgInputs = NewInputs<ISwitcherProgramInputViewModel>();
            _mocks.PrevInputs = NewInputs<ISwitcherPreviewInputViewModel>();
            _mocks.Cut = new Mock<ISwitcherCutButtonViewModel>();
            _mocks.Auto = new Mock<ISwitcherAutoButtonViewModel>();

            _mocks.ServiceSource = new Mock<IServiceSource>();
            _mocks.ServiceSource.Setup(m => m.GetVM<ISwitcherProgramInputViewModel>(It.IsAny<NewViewModelInfo>())).Returns(() => _mocks.ProgInputs[_progInputPos++].Object);
            _mocks.ServiceSource.Setup(m => m.GetVM<ISwitcherPreviewInputViewModel>(It.IsAny<NewViewModelInfo>())).Returns(() => _mocks.PrevInputs[_prevInputPos++].Object);
            _mocks.ServiceSource.Setup(m => m.GetVM<ISwitcherCutButtonViewModel>(It.IsAny<NewViewModelInfo>())).Returns(() => _mocks.Cut.Object);
            _mocks.ServiceSource.Setup(m => m.GetVM<ISwitcherAutoButtonViewModel>(It.IsAny<NewViewModelInfo>())).Returns(() => _mocks.Auto.Object);

            Mock<T>[] NewInputs<T>() where T : class, ISwitcherBusInputViewModel
            {
                var res = new Mock<T>[4];
                for (int i = 0; i < 4; i++)
                {
                    var mock = new Mock<T>();
                    mock.SetupGet(m => m.Base).Returns(_mocks.ModelInputs[i]);
                    res[i] = mock;
                }
                return res;
            }
        }

        SwitcherMixBlockViewModel Create() => new(new(new MixBlockViewModelInfo(_model, 8), _mocks.Parent.Object), _mocks.ServiceSource.Object);

        [TestMethod]
        public void Ctor_General()
        {
            var vm = Create();

            Assert.AreEqual(_mocks.Parent.Object, vm.Parent);
            Assert.AreEqual(_model, vm.BaseBlock);
            Assert.AreEqual(8, vm.Index);

            Assert.IsNotNull(vm.ProgramBus);
            Assert.IsNotNull(vm.PreviewBus);

            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherCutButtonViewModel>(new(null, vm)), Times.Once);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherAutoButtonViewModel>(new(null, vm)), Times.Once);

            Assert.AreEqual(_mocks.Cut.Object, vm.CutButton);
            Assert.AreEqual(_mocks.Auto.Object, vm.AutoButton);
        }

        [TestMethod]
        public void Ctor_ProgramOnly()
        {
            var busInput1 = new SwitcherBusInput(1, "Cam1");
            var busInput2 = new SwitcherBusInput(2, "Cam2");
            _model = SwitcherMixBlock.NewCutBus(new(), busInput1, busInput2);

            var vm = Create();
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherProgramInputViewModel>(new(_model.ProgramInputs[0], vm)), Times.Once);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherProgramInputViewModel>(new(_model.ProgramInputs[1], vm)), Times.Once);

            Assert.AreEqual(2, vm.ProgramBus.Count);
            Assert.AreEqual(_mocks.ProgInputs[0].Object, vm.ProgramBus[0]);
            Assert.AreEqual(_mocks.ProgInputs[1].Object, vm.ProgramBus[1]);
        }

        [TestMethod]
        public void Ctor_NoPreview()
        {
            _model = SwitcherMixBlock.NewCutBus(new());
            var vm = Create();
            Assert.AreEqual(0, vm.PreviewBus.Count);
        }

        [TestMethod]
        public void Ctor_Preview()
        {
            var busInput1 = new SwitcherBusInput(1, "Cam1");
            var busInput2 = new SwitcherBusInput(2, "Cam2");
            _model = SwitcherMixBlock.NewProgPrev(new(), Array.Empty<SwitcherBusInput>(), new SwitcherBusInput[2] { busInput1, busInput2 });

            var vm = Create();
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherPreviewInputViewModel>(new(_model.PreviewInputs![0], vm)), Times.Once);
            _mocks.ServiceSource.Verify(m => m.GetVM<ISwitcherPreviewInputViewModel>(new(_model.PreviewInputs![1], vm)), Times.Once);

            Assert.AreEqual(2, vm.PreviewBus.Count);
            Assert.AreEqual(_mocks.PrevInputs[0].Object, vm.PreviewBus[0]);
            Assert.AreEqual(_mocks.PrevInputs[1].Object, vm.PreviewBus[1]);
        }

        [TestMethod]
        public void MainLabel_ProgramPreview()
        {
            _model = SwitcherMixBlock.NewProgPrev(new());
            Assert.AreEqual("Program", Create().MainLabel);
        }

        [TestMethod]
        public void MainLabel_CutBus()
        {
            _model = SwitcherMixBlock.NewCutBus(new());
            Assert.AreEqual("Cut Bus", Create().MainLabel);
        }

        [TestMethod]
        public void ShowPreview_ProgramPreview()
        {
            _model = SwitcherMixBlock.NewProgPrev(new());
            Assert.IsTrue(Create().ShowPreview);
        }

        [TestMethod]
        public void ShowPreview_CutBus()
        {
            _model = SwitcherMixBlock.NewCutBus(new());
            Assert.IsFalse(Create().ShowPreview);
        }

        [TestMethod]
        public void UpdateValue_ProgPrev()
        {
            _model = SwitcherMixBlock.NewProgPrevSameInputs(new(), _mocks.ModelInputs);
            var vm = Create();
            vm.RefreshBuses(3, 4);

            _mocks.ProgInputs[0].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.ProgInputs[1].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.ProgInputs[1].Verify(m => m.SetHighlight(true), Times.Once);
            _mocks.ProgInputs[2].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.ProgInputs[3].Verify(m => m.SetHighlight(false), Times.Once);

            _mocks.PrevInputs[0].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.PrevInputs[1].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.PrevInputs[2].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.PrevInputs[3].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.PrevInputs[3].Verify(m => m.SetHighlight(true), Times.Once);
        }

        [TestMethod]
        public void UpdateValue_CutBus()
        {
            _model = SwitcherMixBlock.NewCutBus(new(), _mocks.ModelInputs);
            var vm = Create();
            vm.RefreshBuses(3, 4);

            _mocks.ProgInputs[0].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.ProgInputs[1].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.ProgInputs[1].Verify(m => m.SetHighlight(true), Times.Once);
            _mocks.ProgInputs[2].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.ProgInputs[3].Verify(m => m.SetHighlight(false), Times.Once);

            _mocks.PrevInputs[0].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.PrevInputs[1].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.PrevInputs[2].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.PrevInputs[3].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.PrevInputs[3].Verify(m => m.SetHighlight(true), Times.Never);
        }

        [TestMethod]
        public void SetProgram()
        {
            Create().SetProgram(4);
            _mocks.Parent.Verify(m => m.SetValue(8, 0, 4));
        }

        [TestMethod]
        public void SetPreview()
        {
            Create().SetPreview(3);
            _mocks.Parent.Verify(m => m.SetValue(8, 1, 3));
        }

        [TestMethod]
        public void CutButtonPress()
        {
            Create().CutButtonPress();
            _mocks.Parent.Verify(m => m.Cut(8));
        }
    }
}