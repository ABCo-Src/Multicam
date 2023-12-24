using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.Structures;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.General
{
	public interface INamedMovableListItemVM
	{
		string Name { get; set; }
		void OpenGeneralEditMenu(CursorPosition pos);
		void Rename();
		void MoveUp();
		void MoveDown();
		void Delete();
	}

	public abstract partial class NamedMovableBoundListItemVM<TListType, TItemType> : BoundViewModelBase<TItemType>, INamedMovableListItemVM
		where TItemType : INotifyPropertyChanged, INamedServerComponent
		where TListType : IServerList<TItemType>
	{
		readonly Dispatched<TListType> _list;

		[ObservableProperty] string _name = "";

		public NamedMovableBoundListItemVM(Dispatched<TListType> list, Dispatched<TItemType> feature, IClientInfo info) : base(feature, info)
		{
			_list = list;
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

		public void Rename() => throw new NotImplementedException();
		public void MoveUp() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveUp(p));
		public void MoveDown() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveDown(p));
		public void Delete() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.Delete(p));

		protected override void OnServerStateChange(string? changedProp)
		{
			Name = _serverComponent.Get(m => m.Name);
		}
	}
}
