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
    public interface ISwitcherFeatureVM : IFeatureViewModel { }
    public partial class SwitcherFeatureViewModel : FeatureViewModel, ISwitcherFeatureVM
    {
        ISwitcherRunningFeature _model;

        public SwitcherFeatureViewModel(NewViewModelInfo info, IServiceSource serviceSource) : base(serviceSource, (IProjectFeaturesViewModel)info.Parent)
        {
            _model = (ISwitcherRunningFeature)info.Model!;

            var targetSpecs = _model.SwitcherSpecs;
            _mixBlocks = new ObservableCollection<ISwitcherMixBlockVM>();

            for (int i = 0; i < targetSpecs.MixBlocks.Count; i++)
                _mixBlocks.Add(serviceSource.GetVM<ISwitcherMixBlockVM>(new(targetSpecs.MixBlocks[i], this)));
        }

        public override IRunningFeature BaseFeature => _model;
        public override FeatureViewType ContentView => FeatureViewType.Switcher;

        [ObservableProperty] ObservableCollection<ISwitcherMixBlockVM> _mixBlocks;
    }
}