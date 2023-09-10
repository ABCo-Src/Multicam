using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.Core.Features
{
	// Interaction handler for a locally-running feature
	public interface ILocalFeatureInteractionHandler : IFeatureInteractionHandler, IParameteredService<FeatureTypes, FeatureDataInfo[]> { }
	public class LocalFeatureInteractionHandler : ILocalFeatureInteractionHandler, ILocalFragmentCollection
	{
        readonly Data[] _fragmentStore;
        readonly ILiveFeature _runningFeature;
		IFragmentChangeEventHandler? _parentEventHandler;

        public LocalFeatureInteractionHandler(FeatureTypes type, FeatureDataInfo[] fragments, IServiceSource servSource)
        {
			_fragmentStore = fragments.Select(i => new Data(i.Type, i.DefaultValue)).ToArray();
			_runningFeature = servSource.Get<IFeatureContentFactory>().GetLiveFeature(type, this);
		}

		public T GetData<T>() where T : FeatureData => (T)_fragmentStore.First(s => typeof(T).IsAssignableTo(s.Type)).Object;
		public void RefreshData<T>() where T : FeatureData => _parentEventHandler?.OnDataChange(GetData<T>());
		public void SetData(FeatureData newValue)
		{
			var index = GetFragmentIndex(newValue);
			_fragmentStore[index].Object = newValue;
			_parentEventHandler?.OnDataChange(newValue);
		}

		public void PerformAction(int id) => _runningFeature.PerformAction(id);
		public void PerformAction(int id, object structure) => _runningFeature.PerformAction(id, structure);
		public void SetFragmentChangeHandler(IFragmentChangeEventHandler? eventHandler) => _parentEventHandler = eventHandler;

		private int GetFragmentIndex(FeatureData data)
		{
			Type targetType = data.GetType();
			int index = Array.FindIndex(_fragmentStore, s => targetType.IsAssignableTo(s.Type));
			if (index == -1) throw new Exception("Attempt to set unregistered fragment!");
			return index;
		}

		public void Dispose() => _runningFeature.Dispose();

		public struct Data
		{
			public Type Type;
			public FeatureData Object;

			public Data(Type type, FeatureData obj) => (Type, Object) = (type, obj);
		}
	}
}