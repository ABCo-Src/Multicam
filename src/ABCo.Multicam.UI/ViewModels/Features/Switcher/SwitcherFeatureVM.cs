using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features.Switcher;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherFeatureVM : IVMForSwitcherFeature, ILiveFeatureViewModel
    {
    }

    public partial class SwitcherFeatureVM : BindingViewModelBase<IVMForSwitcherFeature>, ISwitcherFeatureVM
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