using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features.Switcher
{
    [TestClass]
    public class SwitcherMixBlockVMTests
    {
        public record struct Mocks(
            Mock<IServiceSource> ServiceSource,
            Mock<ISwitcherFeatureVM> Parent,
            Mock<ISwitcherRunningFeature> RawFeature,
            Mock<ISwitcherCutButtonVM> Cut,
            Mock<ISwitcherAutoButtonVM> Auto,
            Mock<ISwitcherProgramInputVM>[] ProgInputs,
            Mock<ISwitcherPreviewInputVM>[] PrevInputs,
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
            _mocks.RawFeature = new Mock<ISwitcherRunningFeature>();

            _mocks.ModelInputs = new SwitcherBusInput[] { new(2, "abc"), new(3, "ghi"), new(5, "def"), new(4, "kl") };
            _mocks.ProgInputs = NewInputs<ISwitcherProgramInputVM>();
            _mocks.PrevInputs = NewInputs<ISwitcherPreviewInputVM>();
            _mocks.Cut = new Mock<ISwitcherCutButtonVM>();
            _mocks.Auto = new Mock<ISwitcherAutoButtonVM>();

            _mocks.ServiceSource = new Mock<IServiceSource>();
            _mocks.ServiceSource.Setup(m => m.Get<ISwitcherProgramInputVM>()).Returns(() => _mocks.ProgInputs[_progInputPos++].Object);
            _mocks.ServiceSource.Setup(m => m.Get<ISwitcherPreviewInputVM>()).Returns(() => _mocks.PrevInputs[_prevInputPos++].Object);
            _mocks.ServiceSource.Setup(m => m.Get<ISwitcherCutButtonVM>()).Returns(() => _mocks.Cut.Object);
            _mocks.ServiceSource.Setup(m => m.Get<ISwitcherAutoButtonVM>()).Returns(() => _mocks.Auto.Object);

            Mock<T>[] NewInputs<T>() where T : class, ISwitcherBusInputVM
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

        SwitcherMixBlockVM Create() => new(_mocks.ServiceSource.Object)
        {
            RawMixBlock = _model,
            RawMixBlockIndex = 8,
            RawFeature = _mocks.RawFeature.Object,
            Parent = _mocks.Parent.Object
        };

        [TestMethod]
        public void CutButton()
        {
            var vm = Create();
            Assert.AreEqual(_mocks.Cut.Object, vm.CutButton);
            _mocks.Cut.Verify(m => m.FinishConstruction(vm));
            _mocks.ServiceSource.Verify(m => m.Get<ISwitcherCutButtonVM>(), Times.Once);
        }

        [TestMethod]
        public void AutoButton()
        {
            var vm = Create();
            Assert.AreEqual(_mocks.Auto.Object, vm.AutoButton);
            _mocks.Auto.Verify(m => m.FinishConstruction(vm));
            _mocks.ServiceSource.Verify(m => m.Get<ISwitcherAutoButtonVM>(), Times.Once);
        }

        [TestMethod]
        public void RawMixBlock_UpdatesProgramBus()
        {
            _model = SwitcherMixBlock.NewProgPrevSameInputs(new(), _mocks.ModelInputs);
            var vm = Create();

            _progInputPos = 0;
            var busInput1 = new SwitcherBusInput(1, "Cam1");
            var busInput2 = new SwitcherBusInput(2, "Cam2");
            vm.RawMixBlock = SwitcherMixBlock.NewCutBus(new(), busInput1, busInput2);

            var programBus = vm.ProgramBus;

            _mocks.ServiceSource.Verify(m => m.Get<ISwitcherProgramInputVM>());
            _mocks.ProgInputs[0].Verify(m => m.FinishConstruction(busInput1, vm));
            _mocks.ProgInputs[1].Verify(m => m.FinishConstruction(busInput2, vm));

            Assert.AreEqual(2, programBus.Count);
            Assert.AreEqual(_mocks.ProgInputs[0].Object, programBus[0]);
            Assert.AreEqual(_mocks.ProgInputs[1].Object, programBus[1]);
        }

        [TestMethod]
        public void RawMixBlock_UpdatesPreview_NoPreview()
        {
            _model = SwitcherMixBlock.NewProgPrevSameInputs(new(), _mocks.ModelInputs);
            var vm = Create();

            vm.RawMixBlock = SwitcherMixBlock.NewCutBus(new());
            Assert.AreEqual(0, vm.PreviewBus.Count);
        }

        [TestMethod]
        public void RawMixBlock_UpdatesPreviewBus()
        {
            _model = SwitcherMixBlock.NewProgPrevSameInputs(new(), _mocks.ModelInputs);
            var vm = Create();

            _prevInputPos = 0;
            var busInput1 = new SwitcherBusInput(1, "Cam1");
            var busInput2 = new SwitcherBusInput(2, "Cam2");
            vm.RawMixBlock = SwitcherMixBlock.NewProgPrev(new(), Array.Empty<SwitcherBusInput>(), new SwitcherBusInput[2] { busInput1, busInput2 });

            var previewBus = vm.PreviewBus;

            _mocks.ServiceSource.Verify(m => m.Get<ISwitcherPreviewInputVM>());
            _mocks.PrevInputs[0].Verify(m => m.FinishConstruction(busInput1, vm));
            _mocks.PrevInputs[1].Verify(m => m.FinishConstruction(busInput2, vm));

            Assert.AreEqual(2, previewBus.Count);
            Assert.AreEqual(_mocks.PrevInputs[0].Object, previewBus[0]);
            Assert.AreEqual(_mocks.PrevInputs[1].Object, previewBus[1]);
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
        public void RawProgram_Change()
        {
            _model = SwitcherMixBlock.NewProgPrevSameInputs(new(), _mocks.ModelInputs);
            Create().RawProgram = 3;

            _mocks.ProgInputs[0].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.ProgInputs[1].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.ProgInputs[1].Verify(m => m.SetHighlight(true), Times.Once);
            _mocks.ProgInputs[2].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.ProgInputs[3].Verify(m => m.SetHighlight(false), Times.Once);
        }

        [TestMethod]
        public void RawPreview_Change()
        {
            _model = SwitcherMixBlock.NewProgPrevSameInputs(new(), _mocks.ModelInputs);
            Create().RawPreview = 4;

            _mocks.PrevInputs[0].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.PrevInputs[1].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.PrevInputs[2].Verify(m => m.SetHighlight(false), Times.Once);
            _mocks.PrevInputs[3].Verify(m => m.SetHighlight(false), Times.Never);
            _mocks.PrevInputs[3].Verify(m => m.SetHighlight(true), Times.Once);
        }

        [TestMethod]
        public void SetProgram()
        {
            Create().SetProgram(4);
            _mocks.RawFeature.Verify(m => m.PostValue(8, 0, 4));
        }

        [TestMethod]
        public void SetPreview()
        {
            Create().SetPreview(3);
            _mocks.RawFeature.Verify(m => m.PostValue(8, 1, 3));
        }

        [TestMethod]
        public void CutButtonPress()
        {
            Create().CutButtonPress();
            _mocks.RawFeature.Verify(m => m.Cut(8));
        }
    }
}