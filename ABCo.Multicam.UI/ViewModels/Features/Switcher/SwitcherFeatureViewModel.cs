using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
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
    public interface ISwitcherFeatureVM : IVMForSwitcherFeature, ILiveFeatureViewModel
    {
    }

    public partial class SwitcherFeatureViewModel : BindingViewModelBase<IVMForSwitcherFeature>, ISwitcherFeatureVM
    {
        // Synced to the model:
        [ObservableProperty] IVMBinder<IVMForSwitcherMixBlock>[] _rawMixBlocks = null!;
        [ObservableProperty] ISwitcherMixBlockVM[]? _mixBlocks;

        partial void OnRawMixBlocksChanged(IVMBinder<IVMForSwitcherMixBlock>[] value)
        {
            var newArr = new ISwitcherMixBlockVM[value.Length];
            for (int i = 0; i < newArr.Length; i++) newArr[i] = value[i].GetVM<ISwitcherMixBlockVM>(this);
            MixBlocks = newArr;
        }
    }
}