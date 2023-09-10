using ABCo.Multicam.Core.Features.Switchers;
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
	public interface IFeature : IParameteredService<FeatureTypes>, IDisposable
	{
		IFeaturePresenter UIPresenter { get; }
		IFeatureInteractionHandler InteractionHandler { get; }
		void PerformAction(int id);
		void PerformAction(int id, object param);
		void RefreshData<T>() where T : FeatureData;
	}

	public class Feature : IFeature, IFragmentChangeEventHandler
	{
		public IFeatureInteractionHandler InteractionHandler { get; private set; }
		public IFeaturePresenter UIPresenter { get; private set; }

		public static IFeature New(FeatureTypes featureType, IServiceSource servSource) => new Feature(featureType, servSource);
		public Feature(FeatureTypes featureType, IServiceSource servSource)
		{
			// Create the interaction handler
			var fragments = servSource.Get<IFeatureContentFactory>().GetFeatureFragments(featureType);
			InteractionHandler = servSource.Get<ILocalFeatureInteractionHandler, FeatureTypes, FeatureDataInfo[]>(featureType, fragments);
			InteractionHandler.SetFragmentChangeHandler(this);

			// Create the UI presenter
			UIPresenter = servSource.Get<IFeaturePresenter, IFeature, FeatureTypes>(this, featureType);
			UIPresenter.Init();
		}

		public void RefreshData<T>() where T : FeatureData => InteractionHandler.RefreshData<T>();
		public void PerformAction(int id) => InteractionHandler.PerformAction(id);
		public void PerformAction(int id, object param) => InteractionHandler.PerformAction(id, param);
		public void OnDataChange(FeatureData val) => UIPresenter.OnDataChange(val);
		public void Dispose() => InteractionHandler.Dispose();
	}

	public class FeatureGeneralInfo : FeatureData
	{
		public override int DataId => 0;

		public FeatureTypes Type { get; }
		public string Title { get; }

		public FeatureGeneralInfo(FeatureTypes type, string title)
		{
			Type = type;
			Title = title;
		}
	}

	public struct FeatureDataInfo
	{
		public Type Type;
		public FeatureData DefaultValue;

		public FeatureDataInfo(Type type, FeatureData defaultValue) => (Type, DefaultValue) = (type, defaultValue);
	}

	public abstract class FeatureData 
	{ 
		public abstract int DataId { get; }
	}
}