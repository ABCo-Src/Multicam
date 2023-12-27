using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.General;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features
{
	/// <summary>
	/// Manages all the (running) switchers in the current project.
	/// </summary>
	public interface ISwitcherList : IBindableServerComponent<ISwitcherList>, IServerList<ISwitcher>, IDisposable
    {
		void CreateSwitcher();
    }

	public partial class SwitcherList : BindableServerComponent<ISwitcherList>, ISwitcherList
    {
		[ObservableProperty] IReadOnlyList<ISwitcher> _items = Array.Empty<ISwitcher>();

        ReorderableList<ISwitcher> _workingList;
		readonly IServerInfo _info;

		public SwitcherList(IServerInfo info)
        {
            _info = info;
			_workingList = new ReorderableList<ISwitcher>();
			RefreshSwitchersList();
		}

        public void CreateSwitcher()
        {
			_workingList.Add(_info.Factories.Switcher.CreateSwitcher());
			RefreshSwitchersList();
		}

		public void MoveUp(ISwitcher feature)
		{
			_workingList.MoveUp(feature);
			RefreshSwitchersList();
		}

		public void MoveDown(ISwitcher feature)
		{
			_workingList.MoveDown(feature);
			RefreshSwitchersList();
		}

		public void Delete(ISwitcher feature)
		{
			_workingList.Delete(feature);
			RefreshSwitchersList();
		}

		void RefreshSwitchersList() => Items = _workingList.ToArray();

		public void Dispose() => _workingList.Dispose();
	}
}
