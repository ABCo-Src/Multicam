using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switchers
{
	public interface ISwitcherListItemVM : INotifyPropertyChanged, IDisposable
	{
		ISwitcherVM Switcher { get; }
		string Name { get; set; }
		void Rename();
		void MoveUp();
		void MoveDown();
		void Delete();
	}

	public partial class SwitcherListItemVM : BoundViewModelBase<ISwitcher>, ISwitcherListItemVM
	{
		readonly Dispatched<ISwitcherList> _list;
		[ObservableProperty] ISwitcherVM _switcher;
		[ObservableProperty] string _name = "";

		public SwitcherListItemVM(Dispatched<ISwitcherList> list, Dispatched<ISwitcher> feature, IClientInfo info) : base(feature, info)
		{
			_list = list;
			_switcher = new SwitcherVM(feature, info);
			OnServerStateChange(null);
		}

		public void Rename() => _serverComponent.CallDispatched(l => l.Rename(Name));
		public void MoveUp() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveUp(p));
		public void MoveDown() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveDown(p));
		public void Delete() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.Delete(p));

		protected override void OnServerStateChange(string? changedProp) => Name = _serverComponent.Get(m => m.Name);
	}
}
