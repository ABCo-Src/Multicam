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
    public interface ISwitcherFeatureVM : IFeatureViewModel 
    {
        void SetValue(int mixBlock, int bus, int value);
        void Cut(int mixBlock);
    }

    public partial class SwitcherFeatureViewModel : BindingViewModelBase<SwitcherFeatureViewModel>
    {
        // Synced to the model:
        [ObservableProperty][NotifyPropertyChangedFor(nameof(MixBlocks))] IVMBinder<IVMForSwitcherMixBlock>[] _rawMixBlocks = null!;

        public IEnumerable<ISwitcherMixBlockVM> MixBlocks => RawMixBlocks.Select(m => m.GetVM<ISwitcherMixBlockVM>(this));
    }
}