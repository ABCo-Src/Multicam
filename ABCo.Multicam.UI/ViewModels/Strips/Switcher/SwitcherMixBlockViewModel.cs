using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips.Switcher
{
    public interface ISwitcherMixBlockViewModel { }
    public partial class SwitcherMixBlockViewModel : ViewModelBase, ISwitcherMixBlockViewModel
    {
        public readonly SwitcherMixBlock BaseBlock;
        public readonly ISwitcherStripViewModel Parent;

        public bool ShowPreview => BaseBlock.NativeType == SwitcherBusInputType.ProgramPreview;
        public string MainLabel => BaseBlock.NativeType == SwitcherBusInputType.CutBus ? "Cut Bus" : "Program";

        [ObservableProperty] ObservableCollection<SwitcherBusInputViewModel> _programBus;
        [ObservableProperty] ObservableCollection<SwitcherBusInputViewModel> _previewBus;

        [ObservableProperty] SwitcherActButtonViewModel _cutButton;
        [ObservableProperty] SwitcherActButtonViewModel _autoButton;

        public SwitcherMixBlockViewModel(SwitcherMixBlock model, IServiceSource source, ISwitcherStripViewModel parent)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            Parent = parent;
            BaseBlock = model;

            _programBus = new ObservableCollection<SwitcherBusInputViewModel>();
            _previewBus = new ObservableCollection<SwitcherBusInputViewModel>();

            // Add program bus inputs
            for (int i = 0; i < model.ProgramInputs.Count; i++)
                _programBus.Add(new SwitcherBusInputViewModel(model.ProgramInputs[i], true, source, this));

            // Add preview bus inputs
            if (model.PreviewInputs != null)
                for (int i = 0; i < model.PreviewInputs.Count; i++)
                    _previewBus.Add(new SwitcherBusInputViewModel(model.PreviewInputs[i], false, source, this));

            _cutButton = new SwitcherActButtonViewModel(SwitcherActButtonViewModel.Type.Cut, source, this);
            _autoButton = new SwitcherActButtonViewModel(SwitcherActButtonViewModel.Type.Auto, source, this);
        }
    }
}