using ABCo.Multicam.Server.Features.Switchers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features
{
	/// <summary>
	/// Manages all the (running) switchers in the current project.
	/// </summary>
	public interface ISwitcherList : IBindableServerComponent<ISwitcherList>, IDisposable
    {
		IReadOnlyList<ISwitcher> Features { get; }

		void CreateSwitcher();
        void MoveUp(ISwitcher feature);
        void MoveDown(ISwitcher feature);
        void Delete(ISwitcher feature);
    }

	public partial class SwitcherList : BindableServerComponent<ISwitcherList>, ISwitcherList
    {
		[ObservableProperty] IReadOnlyList<ISwitcher> _features = Array.Empty<ISwitcher>();

        List<ISwitcher> _workingList;
		readonly IServerInfo _info;

		public SwitcherList(IServerInfo info)
        {
            _info = info;
			_workingList = new List<ISwitcher>();
			RefreshFeaturesList();
		}

        public void CreateSwitcher()
        {
			_workingList.Add(_info.Get<ISwitcher>());
			RefreshFeaturesList();
		}

        public void MoveUp(ISwitcher feature)
        {
            int indexOfFeature = _workingList.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_workingList[indexOfFeature], _workingList[indexOfFeature - 1]) = (_workingList[indexOfFeature - 1], _workingList[indexOfFeature]);

            RefreshFeaturesList();
        }

        public void MoveDown(ISwitcher feature)
        {
            int indexOfFeature = _workingList.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _workingList.Count - 1) return;

            (_workingList[indexOfFeature], _workingList[indexOfFeature + 1]) = (_workingList[indexOfFeature + 1], _workingList[indexOfFeature]);

			RefreshFeaturesList();
		}

        public void Delete(ISwitcher feature)
        {
            _workingList.Remove(feature);
            feature.Dispose();

			RefreshFeaturesList();
		}

		public void Dispose()
		{
			for (int i = 0; i < _workingList.Count; i++)
				_workingList[i].Dispose();
		}

		void RefreshFeaturesList() => Features = _workingList.ToArray();
	}
}
