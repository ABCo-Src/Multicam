using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.Structures;
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
		void OpenGeneralEditMenu(CursorPosition pos);
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

		public void OpenGeneralEditMenu(CursorPosition pos) => _info.Shared.PopOut.OpenContext(new ContextMenuDetails("", OnMenuOptionChoose, null, pos, new string[]
		{
			"Rename",
			"Move Up",
			"Move Down",
			"Delete"
		}));

		void OnMenuOptionChoose(string s)
		{
			switch (s)
			{
				case "Rename":
					Rename();
					break;
				case "Move Up":
					MoveUp();
					break;
				case "Move Down":
					MoveDown();
					break;
				case "Delete":
					Delete();
					break;
			}
		}

		public void Rename() => _serverComponent.CallDispatched(l => l.Rename(Name));
		public void MoveUp() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveUp(p));
		public void MoveDown() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveDown(p));
		public void Delete() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.Delete(p));

		protected override void OnServerStateChange(string? changedProp) => Name = _serverComponent.Get(m => m.Name);
	}
}
