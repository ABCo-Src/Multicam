using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
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
    public interface ISwitcherFeatureVM : IFeatureViewModel 
    {
        void SetValue(int mixBlock, int bus, int value);
        void Cut(int mixBlock);
    }

    public partial class SwitcherFeatureViewModel : FeatureViewModel, ISwitcherFeatureVM
    {
        ISwitcherRunningFeature _model;

        public SwitcherFeatureViewModel(NewViewModelInfo info, IServiceSource serviceSource) : base(serviceSource, (IProjectFeaturesViewModel)info.Parent)
        {
            _model = (ISwitcherRunningFeature)info.Model!;
            _model.SetOnBusChangeFinishForVM(OnBusChangeFinish);

            var targetSpecs = _model.SwitcherSpecs;
            _mixBlocks = new ObservableCollection<ISwitcherMixBlockVM>();

            for (int i = 0; i < targetSpecs.MixBlocks.Count; i++)
            {
                var newVM = serviceSource.GetVM<ISwitcherMixBlockVM>(new(new MixBlockViewModelInfo(targetSpecs.MixBlocks[i], i), this));
                _mixBlocks.Add(newVM);
                newVM.RefreshBuses(_model.GetValue(i, 0), _model.GetValue(i, 1));
            }
        }

        public override IRunningFeature BaseFeature => _model;
        public override FeatureViewType ContentView => FeatureViewType.Switcher;

        [ObservableProperty] ObservableCollection<ISwitcherMixBlockVM> _mixBlocks;

        public void OnBusChangeFinish(RetrospectiveFadeInfo? info)
        {
            for (int i = 0; i < MixBlocks.Count; i++)
                MixBlocks[i].RefreshBuses(_model.GetValue(i, 0), _model.GetValue(i, 1));
        }

        public void SetValue(int mixBlock, int bus, int value) => _model.PostValue(mixBlock, bus, value);

        public void Cut(int mixBlock) => _model.Cut(mixBlock);
    }
}