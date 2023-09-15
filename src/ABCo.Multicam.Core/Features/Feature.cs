using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Interaction;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.Core.Features
{
	public interface IFeaturePresenter : IParameteredService<IFeature, FeatureTypes>
	{
		void Init();
		void OnDataChange(FeatureData data);
	}

	/// <summary>
	/// Represents a feature currently loaded, whether running locally on this system or remotely.
	/// Introduces the entire data fragments system, as well as fragments like the "Title" or "Item" within there
	/// </summary>
	public interface IFeature : IParameteredService<FeatureTypes, IFeatureDataSource, IFeatureActionTarget>, IDisposable
	{
		IFeaturePresenter UIPresenter { get; }
		void PerformAction(int id);
		void PerformAction(int id, object param);
		void RefreshData<T>() where T : FeatureData;
	}

	public class Feature : IFeature, IFeatureDataChangeEventHandler
	{
		public readonly IFeatureDataSource _dataSource; // The raw data source. May be local or may be remote.
		public readonly IFeatureActionTarget _actionTarget; // The target to direct actions towards. May go to a locally-running feature, or may be remote.

		public IFeaturePresenter UIPresenter { get; private set; }

		public Feature(FeatureTypes featureType, IFeatureDataSource dataSource, IFeatureActionTarget actionTarget, IServiceSource servSource)
		{
			_dataSource = dataSource;
			_dataSource.SetDataChangeHandler(this);
			_actionTarget = actionTarget;

			// Create the UI presenter
			UIPresenter = servSource.Get<IFeaturePresenter, IFeature, FeatureTypes>(this, featureType);
			UIPresenter.Init();
		}

		public void RefreshData<T>() where T : FeatureData => _dataSource.RefreshData<T>();
		public void PerformAction(int id) => _actionTarget.PerformAction(id);
		public void PerformAction(int id, object param) => _actionTarget.PerformAction(id, param);
		public void OnDataChange(FeatureData val) => UIPresenter.OnDataChange(val);

		public void Dispose() => _actionTarget.Dispose();
	}
}