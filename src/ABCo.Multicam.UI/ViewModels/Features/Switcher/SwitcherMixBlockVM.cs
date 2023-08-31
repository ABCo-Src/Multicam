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
		ISwitcherProgramInputVM[] ProgramBus { get; }
		ISwitcherPreviewInputVM[] PreviewBus { get; }
		void SetProgram(int value);
        void SetPreview(int value);
        void CutButtonPress();
    }

    public partial class SwitcherMixBlockVM : BindingViewModelBase<IVMForSwitcherMixBlock>, ISwitcherMixBlockVM
    {
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

        [ObservableProperty] ISwitcherProgramInputVM[] _programBus = Array.Empty<ISwitcherProgramInputVM>();
        [ObservableProperty] ISwitcherPreviewInputVM[] _previewBus = Array.Empty<ISwitcherPreviewInputVM>();

        [ObservableProperty] ISwitcherCutButtonVM _cutButton;
        [ObservableProperty] ISwitcherAutoButtonVM _autoButton;

        public SwitcherMixBlockVM(IServiceSource source)
        {
            _servSource = source;

            _cutButton = source.Get<ISwitcherCutButtonVM>();
            _cutButton.FinishConstruction(this);

            _autoButton = source.Get<ISwitcherAutoButtonVM>();
            _autoButton.FinishConstruction(this);
        }

        void InvalidateProgramBus()
        {
            ProgramBus = new ISwitcherProgramInputVM[4];
            for (int i = 0; i < RawMixBlock.ProgramInputs.Count; i++)
            {
                var newVM = _servSource.Get<ISwitcherProgramInputVM>();
                newVM.FinishConstruction(RawMixBlock.ProgramInputs[i], this);
                ProgramBus[i] = newVM;
            }
        }

        void InvalidatePreviewBus()
        {
			PreviewBus = new ISwitcherPreviewInputVM[4];
			if (RawMixBlock.PreviewInputs == null) return;

            for (int i = 0; i < RawMixBlock.PreviewInputs.Count; i++)
            {
                var newVM = _servSource.Get<ISwitcherPreviewInputVM>();
                newVM.FinishConstruction(RawMixBlock.PreviewInputs[i], this);
                PreviewBus[i] = newVM;
            }
        }

        void RefreshProgram()
        {
            for (int i = 0; i < ProgramBus.Length; i++)
                ProgramBus[i].SetHighlight(ProgramBus[i].Base.Id == RawProgram);
        }

        void RefreshPreview()
        {
            for (int i = 0; i < PreviewBus.Length; i++)
                PreviewBus[i].SetHighlight(PreviewBus[i].Base.Id == RawPreview);
        }

        public void SetProgram(int value) => RawFeature.SendProgram(RawMixBlockIndex, value);
        public void SetPreview(int value) => RawFeature.SendPreview(RawMixBlockIndex, value);
        public void CutButtonPress() => RawFeature.Cut(RawMixBlockIndex);

    }
}