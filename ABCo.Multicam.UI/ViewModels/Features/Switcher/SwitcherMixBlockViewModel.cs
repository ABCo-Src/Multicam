using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
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
    public interface ISwitcherMixBlockVM 
    {
        void UpdateValue(int program, int preview);
    }

    public partial class SwitcherMixBlockViewModel : ViewModelBase, ISwitcherMixBlockVM
    {
        public readonly SwitcherMixBlock BaseBlock;
        public readonly ISwitcherFeatureVM Parent;

        public bool ShowPreview => BaseBlock.NativeType == SwitcherMixBlockType.ProgramPreview;
        public string MainLabel => BaseBlock.NativeType == SwitcherMixBlockType.CutBus ? "Cut Bus" : "Program";

        [ObservableProperty] ObservableCollection<ISwitcherProgramInputViewModel> _programBus;
        [ObservableProperty] ObservableCollection<ISwitcherPreviewInputViewModel> _previewBus;

        [ObservableProperty] ISwitcherCutButtonViewModel _cutButton;
        [ObservableProperty] ISwitcherAutoButtonViewModel _autoButton;

        public SwitcherMixBlockViewModel(NewViewModelInfo info, IServiceSource source)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            var model = (SwitcherMixBlock)info.Model!;

            Parent = (ISwitcherFeatureVM)info.Parent;
            BaseBlock = model;

            _programBus = new();
            _previewBus = new();

            // Add program bus inputs
            for (int i = 0; i < model.ProgramInputs.Count; i++)
                _programBus.Add(source.GetVM<ISwitcherProgramInputViewModel>(new(model.ProgramInputs[i], this)));

            // Add preview bus inputs
            if (model.PreviewInputs != null)
                for (int i = 0; i < model.PreviewInputs.Count; i++)
                    _previewBus.Add(source.GetVM<ISwitcherPreviewInputViewModel>(new(model.PreviewInputs[i], this)));

            _cutButton = source.GetVM<ISwitcherCutButtonViewModel>(new(null, this));
            _autoButton = source.GetVM<ISwitcherAutoButtonViewModel>(new(null, this));
        }

        public void UpdateValue(int program, int preview) 
        {
            for (int i = 0; i < ProgramBus.Count; i++)
                ProgramBus[i].SetHighlight(ProgramBus[i].Base.Id == program);
            for (int i = 0; i < PreviewBus.Count; i++)
                PreviewBus[i].SetHighlight(PreviewBus[i].Base.Id == preview);
        }
    }
}