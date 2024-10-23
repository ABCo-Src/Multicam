using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.Structures;
using ABCo.Multicam.Client.ViewModels.General;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switchers
{
    public interface ISwitcherListItemVM : INamedMovableListItemVM<ISwitcherVM>, INotifyPropertyChanged, IDisposable
    {
    }

    public partial class SwitcherListItemVM : NamedMovableBoundListItemVM<ISwitcherList, ISwitcher, ISwitcherVM>, ISwitcherListItemVM
    {
        public SwitcherListItemVM(Dispatched<ISwitcherList> list, Dispatched<ISwitcher> feature, IFrameClientInfo info) 
            : base(list, feature, new SwitcherVM(feature, info), info) { }
    }
}
