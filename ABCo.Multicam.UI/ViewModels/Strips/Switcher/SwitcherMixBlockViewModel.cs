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

        [ObservableProperty] ObservableCollection<SwitcherButtonViewModel> _programBus;
        [ObservableProperty] ObservableCollection<SwitcherButtonViewModel> _previewBus;

        [ObservableProperty] SwitcherButtonViewModel _cutButton;
        [ObservableProperty] SwitcherButtonViewModel _autoButton;

        public SwitcherMixBlockViewModel(SwitcherMixBlock model, IServiceSource source, ISwitcherStripViewModel parent)
        {
            if (source == null) throw new ServiceSourceNotGivenException();

            BaseBlock = model;
            Parent = parent;
            _programBus = new ObservableCollection<SwitcherButtonViewModel>()
            {
                new SwitcherButtonViewModel(source, this, "Cam1") { Status = SwitcherButtonStatus.ProgramActive },
                new SwitcherButtonViewModel(source, this, "Cam2"),
                new SwitcherButtonViewModel(source, this, "Cam3"),
                new SwitcherButtonViewModel(source, this, "Cam4")
            };

            _previewBus = new ObservableCollection<SwitcherButtonViewModel>()
            {
                new SwitcherButtonViewModel(source, this, "Cam1"),
                new SwitcherButtonViewModel(source, this, "Cam2") { Status = SwitcherButtonStatus.PreviewActive },
                new SwitcherButtonViewModel(source, this, "Cam3"),
                new SwitcherButtonViewModel(source, this, "Cam4")
            };

            _cutButton = new SwitcherButtonViewModel(source, this, "Cut");
            _autoButton = new SwitcherButtonViewModel(source, this, "Auto");
        }
    }
}