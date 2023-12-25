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
	public interface INamedMovableListItemVM : INotifyPropertyChanged
	{
		string Name { get; set; }
		bool IsEditingName { get; set; }
		void OpenGeneralEditMenu(CursorPosition pos);
		void StartRename();
		void OnFinishRename();
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
		[ObservableProperty] bool _isEditingName = false;

		public NamedMovableBoundListItemVM(Dispatched<TListType> list, Dispatched<TItemType> feature, IFrameClientInfo info) : base(feature, info)
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
					StartRename();
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

		public void StartRename() => IsEditingName = true;
		public void OnFinishRename()
		{
			_serverComponent.CallDispatched(m => m.Rename(Name));
			IsEditingName = false;
		}
		public void MoveUp() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveUp(p));
		public void MoveDown() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.MoveDown(p));
		public void Delete() => _list.CallDispatchedAndUnpack(_serverComponent, (l, p) => l.Delete(p));

		protected override void OnServerStateChange(string? changedProp)
		{
			Name = _serverComponent.Get(m => m.Name);
		}
	}
}
