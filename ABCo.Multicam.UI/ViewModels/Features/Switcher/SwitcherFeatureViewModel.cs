using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherFeatureViewModel : IFeatureViewModel { }
    public partial class SwitcherFeatureViewModel : FeatureViewModel, ISwitcherFeatureViewModel
    {
        ISwitcherRunningFeature _model;

        public SwitcherFeatureViewModel(FeatureViewModelInfo info, IServiceSource serviceSource) : base(serviceSource, info.Parent)
        {
            _model = (ISwitcherRunningFeature)info.Feature;

            var targetSpecs = _model.SwitcherSpecs;
            _mixBlocks = new ObservableCollection<SwitcherMixBlockViewModel>();

            for (int i = 0; i < targetSpecs.MixBlocks.Count; i++)
                _mixBlocks.Add(new SwitcherMixBlockViewModel(targetSpecs.MixBlocks[i], serviceSource, this));
        }

        public override IRunningFeature BaseFeature => _model;
        public override FeatureViewType ContentView => FeatureViewType.Switcher;

        [ObservableProperty] ObservableCollection<SwitcherMixBlockViewModel> _mixBlocks;
    }
}