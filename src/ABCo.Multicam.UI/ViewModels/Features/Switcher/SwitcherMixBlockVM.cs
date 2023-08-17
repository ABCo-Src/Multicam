using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherMixBlockVM : IVMForSwitcherMixBlock
    {
        void SetProgram(int value);
        void SetPreview(int value);
        void CutButtonPress();
    }

    public partial class SwitcherMixBlockVM : BindingViewModelBase<IVMForSwitcherMixBlock>, ISwitcherMixBlockVM
    {
        public static List<object> Test { get; set; } = new();

        IServiceSource _servSource;

        // Synced to the model: 
        [ObservableProperty] int _rawMixBlockIndex;
        [ObservableProperty] ISwitcherRunningFeature _rawFeature = null!;
        [ObservableProperty] SwitcherMixBlock _rawMixBlock = null!;
        [ObservableProperty] int _rawProgram;
        [ObservableProperty] int _rawPreview;

        partial void OnRawMixBlockChanged(SwitcherMixBlock value)
        {
            InvalidateProgramBus();
            InvalidatePreviewBus();
        }

        partial void OnRawProgramChanged(int value) => RefreshProgram();
        partial void OnRawPreviewChanged(int value) => RefreshPreview();

        public bool ShowPreview => RawMixBlock.NativeType == SwitcherMixBlockType.ProgramPreview;
        public string MainLabel => RawMixBlock.NativeType == SwitcherMixBlockType.CutBus ? "Cut Bus" : "Program";

        [ObservableProperty] ObservableCollection<ISwitcherProgramInputVM> _programBus;
        [ObservableProperty] ObservableCollection<ISwitcherPreviewInputVM> _previewBus;

        [ObservableProperty] ISwitcherCutButtonVM _cutButton;
        [ObservableProperty] ISwitcherAutoButtonVM _autoButton;

        public SwitcherMixBlockVM(IServiceSource source)
        {
            _servSource = source;
            _programBus = new();
            _previewBus = new();

            _cutButton = source.Get<ISwitcherCutButtonVM>();
            _cutButton.FinishConstruction(this);

            Test.Add(_cutButton);

            _autoButton = source.Get<ISwitcherAutoButtonVM>();
            _autoButton.FinishConstruction(this);
        }

        void InvalidateProgramBus()
        {
            ProgramBus.Clear();
            for (int i = 0; i < RawMixBlock.ProgramInputs.Count; i++)
            {
                var newVM = _servSource.Get<ISwitcherProgramInputVM>();
                newVM.FinishConstruction(RawMixBlock.ProgramInputs[i], this);
                ProgramBus.Add(newVM);
            }
        }

        void InvalidatePreviewBus()
        {
            PreviewBus.Clear();
            if (RawMixBlock.PreviewInputs == null) return;

            for (int i = 0; i < RawMixBlock.PreviewInputs.Count; i++)
            {
                var newVM = _servSource.Get<ISwitcherPreviewInputVM>();
                newVM.FinishConstruction(RawMixBlock.PreviewInputs[i], this);
                PreviewBus.Add(newVM);
            }
        }

        void RefreshProgram()
        {
            for (int i = 0; i < ProgramBus.Count; i++)
                ProgramBus[i].SetHighlight(ProgramBus[i].Base.Id == RawProgram);
        }

        void RefreshPreview()
        {
            for (int i = 0; i < PreviewBus.Count; i++)
                PreviewBus[i].SetHighlight(PreviewBus[i].Base.Id == RawPreview);
        }

        public void SetProgram(int value) => RawFeature.PostValue(RawMixBlockIndex, 0, value);
        public void SetPreview(int value) => RawFeature.PostValue(RawMixBlockIndex, 1, value);
        public void CutButtonPress() => RawFeature.Cut(RawMixBlockIndex);

    }
}