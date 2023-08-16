using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherMixBlockVM : IVMForSwitcherMixBlock
    {
        void SetProgram(int value);
        void SetPreview(int value);
        void CutButtonPress();
    }

    public partial class SwitcherMixBlockViewModel : BindingViewModelBase<IVMForSwitcherMixBlock>, ISwitcherMixBlockVM
    {
        IServiceSource _servSource;

        // Synced to the model: 
        [ObservableProperty] int _rawMixBlockIndex;
        [ObservableProperty] ISwitcherRunningFeature _rawFeature = null!;
        SwitcherMixBlock _rawMixBlock = null!;
        int _rawProgram;
        int _rawPreview;

        public SwitcherMixBlock RawMixBlock
        {
            get => _rawMixBlock;
            set
            {
                if (SetProperty(ref _rawMixBlock, value))
                {
                    InvalidateProgramBus();
                    InvalidatePreviewBus();
                }
            }
        }

        public int RawProgram
        {
            get => _rawProgram;
            set
            {
                if (SetProperty(ref _rawProgram, value))
                    RefreshProgram();
            }
        }

        public int RawPreview
        {
            get => _rawPreview;
            set
            {
                if (SetProperty(ref _rawPreview, value))
                    RefreshPreview();
            }
        }

        public bool ShowPreview => RawMixBlock.NativeType == SwitcherMixBlockType.ProgramPreview;
        public string MainLabel => RawMixBlock.NativeType == SwitcherMixBlockType.CutBus ? "Cut Bus" : "Program";

        [ObservableProperty] ObservableCollection<ISwitcherProgramInputViewModel> _programBus;
        [ObservableProperty] ObservableCollection<ISwitcherPreviewInputViewModel> _previewBus;

        [ObservableProperty] ISwitcherCutButtonViewModel _cutButton;
        [ObservableProperty] ISwitcherAutoButtonViewModel _autoButton;

        public SwitcherMixBlockViewModel(IServiceSource source)
        {
            _servSource = source;
            _programBus = new();
            _previewBus = new();

            _cutButton = source.Get<ISwitcherCutButtonViewModel>();
            _cutButton.FinishConstruction(this);
            _autoButton = source.Get<ISwitcherAutoButtonViewModel>();
            _autoButton.FinishConstruction(this);
        }

        void InvalidateProgramBus()
        {
            ProgramBus.Clear();
            for (int i = 0; i < _rawMixBlock.ProgramInputs.Count; i++)
            {
                var newVM = _servSource.Get<ISwitcherProgramInputViewModel>();
                newVM.FinishConstruction(_rawMixBlock.ProgramInputs[i], this);
                ProgramBus.Add(newVM);
            }
        }

        void InvalidatePreviewBus()
        {
            PreviewBus.Clear();
            if (_rawMixBlock.PreviewInputs == null) return;

            for (int i = 0; i < _rawMixBlock.PreviewInputs.Count; i++)
            {
                var newVM = _servSource.Get<ISwitcherPreviewInputViewModel>();
                newVM.FinishConstruction(_rawMixBlock.PreviewInputs[i], this);
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