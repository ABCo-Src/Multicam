using ABCo.Multicam.Server.General;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Automation.Buttons
{
    public interface IScriptButtonList : IServerList<IScriptButton>, IBindableServerComponent<IScriptButtonList>
    {
        void CreateAutomation();
    }

    public partial class ScriptButtonList : BindableServerComponent<IScriptButtonList>, IScriptButtonList
    {
        readonly IServerInfo _info;
        readonly ReorderableList<IScriptButton> _workingList;

        [ObservableProperty] IReadOnlyList<IScriptButton> _items = Array.Empty<IScriptButton>();

        public ScriptButtonList(IServerInfo info)
        {
            _info = info;
            _workingList = new ReorderableList<IScriptButton>();
            RefreshList();
        }

        public void CreateAutomation()
        {
            _workingList.Add(_info.Factories.Scripting.CreateButton());
            RefreshList();
        }

        public void MoveUp(IScriptButton feature)
        {
            _workingList.MoveUp(feature);
            RefreshList();
        }

        public void MoveDown(IScriptButton feature)
        {
            _workingList.MoveDown(feature);
            RefreshList();
        }

        public void Delete(IScriptButton feature)
        {
            _workingList.Delete(feature);
            RefreshList();
        }

        void RefreshList() => Items = _workingList.ToArray();
    }
}
