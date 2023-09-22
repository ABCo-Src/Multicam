using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Interaction;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.Core.Features
{
	public interface IFeaturePresenter : IParameteredService<IFeature, IScopeInfo>, IUIPresenter
	{
	}

	/// <summary>
	/// Represents a feature currently loaded, whether running locally on this system or remotely.
	/// Introduces the entire data fragments system, as well as fragments like the "Title" or "Item" within there
	/// </summary>
	public interface IFeature : IParameteredService<FeatureTypes, IFeatureDataSource, IFeatureActionTarget>, IDisposable
	{
        IScopedPresenterStore<IFeature> UIPresenters { get; }
        void PerformAction(int id);
		void PerformAction(int id, object param);
		void RefreshData<T>() where T : FeatureData;
	}

	public class Feature : IFeature, IFeatureDataChangeEventHandler
	{
		readonly IFeatureDataSource _dataSource; // The raw data source. May be local or may be remote.
		readonly IFeatureActionTarget _actionTarget; // The target to direct actions towards. May go to a locally-running feature, or may be remote.

		public IScopedPresenterStore<IFeature> UIPresenters { get; }

		public Feature(FeatureTypes featureType, IFeatureDataSource dataSource, IFeatureActionTarget actionTarget, IServiceSource servSource)
		{
			_dataSource = dataSource;
			_dataSource.SetDataChangeHandler(this);
			_actionTarget = actionTarget;

            // Create the UI presenter
            UIPresenters = servSource.Get<IScopedPresenterStoreFactory>().Get((IFeature)this);
		}

		public void RefreshData<T>() where T : FeatureData => _dataSource.RefreshData<T>();
		public void PerformAction(int id) => _actionTarget.PerformAction(id);
		public void PerformAction(int id, object param) => _actionTarget.PerformAction(id, param);
		public void OnDataChange(FeatureData val) => UIPresenters.OnDataChange(val);

		public void Dispose() => _actionTarget.Dispose();
	}
}