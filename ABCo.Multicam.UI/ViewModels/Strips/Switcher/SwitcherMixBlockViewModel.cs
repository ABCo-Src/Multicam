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

        [ObservableProperty] ObservableCollection<SwitcherBusInputViewModel> _programBus;
        [ObservableProperty] ObservableCollection<SwitcherBusInputViewModel> _previewBus;

        [ObservableProperty] SwitcherBusInputViewModel _cutButton;
        [ObservableProperty] SwitcherBusInputViewModel _autoButton;

        public SwitcherMixBlockViewModel(SwitcherMixBlock model, IServiceSource source, ISwitcherStripViewModel parent)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            Parent = parent;
            BaseBlock = model;

            _programBus = new ObservableCollection<SwitcherBusInputViewModel>();
            for (int i = 0; i < model.ProgramInputs.Count; i++)
                _programBus.Add(new SwitcherBusInputViewModel(model.ProgramInputs[i], true, source, this));

            _previewBus = new ObservableCollection<SwitcherBusInputViewModel>();
            if (model.PreviewInputs !=  null)
            {
                for (int i = 0; i < model.PreviewInputs.Count; i++)
                    _previewBus.Add(new SwitcherBusInputViewModel(model.PreviewInputs[i], false, source, this));
            }
        }
    }
}