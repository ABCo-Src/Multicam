using ABCo.Multicam.Client.ViewModels.Features;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features
{
	/// <summary>
	/// Manages all the (running) features in the current project.
	/// </summary>
	public interface IMainFeatureCollection : IBindableServerComponent<IMainFeatureCollection>, IDisposable
    {
		IReadOnlyList<IFeature> Features { get; }

		void CreateFeature(FeatureTypes type);
        void MoveUp(IFeature feature);
        void MoveDown(IFeature feature);
        void Delete(IFeature feature);
    }

    public partial class ServerFeatures : BindableServerComponent<IMainFeatureCollection>, IMainFeatureCollection
    {
		[ObservableProperty] IReadOnlyList<IFeature> _features = Array.Empty<IFeature>();

        List<IFeature> _workingList;
		readonly IServerInfo _info;

		public ServerFeatures(IServerInfo info) : base(info)
        {
            _info = info;
			_workingList = new List<IFeature>();
			RefreshFeaturesList();
		}

        public void CreateFeature(FeatureTypes type)
        {
			_workingList.Add(_info.Get<IFeatureContentFactory>().GetLiveFeature(type));
			RefreshFeaturesList();
		}

        public void MoveUp(IFeature feature)
        {
            int indexOfFeature = _workingList.IndexOf(feature);

            // Don't do anything if it's at the start
            if (indexOfFeature == 0) return;

            (_workingList[indexOfFeature], _workingList[indexOfFeature - 1]) = (_workingList[indexOfFeature - 1], _workingList[indexOfFeature]);

            RefreshFeaturesList();
        }

        public void MoveDown(IFeature feature)
        {
            int indexOfFeature = _workingList.IndexOf(feature);

            // Don't do anything if it's at the end
            if (indexOfFeature == _workingList.Count - 1) return;

            (_workingList[indexOfFeature], _workingList[indexOfFeature + 1]) = (_workingList[indexOfFeature + 1], _workingList[indexOfFeature]);

			RefreshFeaturesList();
		}

        public void Delete(IFeature feature)
        {
            _workingList.Remove(feature);
            feature.Dispose();

			RefreshFeaturesList();
		}

		public override void DisposeComponent()
		{
			for (int i = 0; i < _workingList.Count; i++)
				_workingList[i].Dispose();
		}

        void RefreshFeaturesList() => Features = _workingList.ToArray();
	}
}
