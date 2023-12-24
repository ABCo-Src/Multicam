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
	public interface ISwitcherListItemVM : INamedMovableListItemVM, INotifyPropertyChanged, IDisposable
	{
		ISwitcherVM Switcher { get; }
	}

	public partial class SwitcherListItemVM : NamedMovableBoundListItemVM<ISwitcherList, ISwitcher>, ISwitcherListItemVM
	{
		[ObservableProperty] ISwitcherVM _switcher;

		public SwitcherListItemVM(Dispatched<ISwitcherList> list, Dispatched<ISwitcher> feature, IClientInfo info) : base(list, feature, info) =>
			_switcher = new SwitcherVM(feature, info);
	}
}
