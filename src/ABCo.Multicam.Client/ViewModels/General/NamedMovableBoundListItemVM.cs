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
    public interface INamedMovableListItemVM<TContent> : INotifyPropertyChanged
    {
        string Name { get; set; }
        bool IsEditingName { get; set; }
        bool IsExpanded { get; set; }
        TContent Content { get; }
        void OpenGeneralEditMenu(CursorPosition pos);
        void StartRename();
        void OnFinishRename();
        void MoveUp();
        void MoveDown();
        void Delete();
        void ToggleExpansion();
    }

    public abstract partial class NamedMovableBoundListItemVM<TList, TItem, TContent> : BoundViewModelBase<TItem>, INamedMovableListItemVM<TContent>
        where TItem : INotifyPropertyChanged, INamedServerComponent
        where TList : IServerList<TItem>
    {
        readonly Dispatched<TList> _list;

        [ObservableProperty] string _name = "";
        [ObservableProperty] TContent _content;
        [ObservableProperty] bool _isEditingName = false;
        [ObservableProperty] bool _isExpanded = false;

        public NamedMovableBoundListItemVM(Dispatched<TList> list, Dispatched<TItem> feature, TContent content, IFrameClientInfo info) : base(feature, info)
        {
            Content = content;
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
        public void ToggleExpansion() => IsExpanded = !IsExpanded;

        protected override void OnServerStateChange(string? changedProp)
        {
            Name = _serverComponent.Get(m => m.Name);
        }
    }
}
