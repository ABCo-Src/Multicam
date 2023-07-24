using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Strips;
using ABCo.Multicam.Core.Strips.Switchers;
using ABCo.Multicam.Core.Strips.Switchers.Types;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips.Switcher
{
    public interface ISwitcherStripViewModel { }
    public partial class SwitcherStripViewModel : StripViewModel, ISwitcherStripViewModel
    {
        ISwitcherRunningStrip _model;

        public SwitcherStripViewModel(ISwitcherRunningStrip model, IServiceSource serviceSource, IProjectStripsViewModel parent) : base(serviceSource, parent)
        {
            _model = model;

            var targetSpecs = _model.SwitcherSpecs;
            _mixBlocks = new ObservableCollection<SwitcherMixBlockViewModel>();

            for (int i = 0; i < targetSpecs.MixBlocks.Count; i++)
                _mixBlocks.Add(new SwitcherMixBlockViewModel(targetSpecs.MixBlocks[i], serviceSource, this));
        }

        public override IRunningStrip BaseStrip => _model;
        public override StripViewType ContentView => StripViewType.Switcher;

        [ObservableProperty] ObservableCollection<SwitcherMixBlockViewModel> _mixBlocks;
    }
}