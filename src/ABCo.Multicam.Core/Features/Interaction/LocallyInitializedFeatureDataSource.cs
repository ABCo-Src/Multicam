using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.Core.Features
{
	public interface ILocallyInitializedFeatureDataSource : IFeatureDataSource, IInstantRetrievalDataSource, IParameteredService<FeatureDataInfo[]> { }
	public class LocallyInitializedFeatureDataSource : ILocallyInitializedFeatureDataSource
	{
        readonly Data[] _fragmentStore;
		IFeatureDataChangeEventHandler? _parentEventHandler;

		public LocallyInitializedFeatureDataSource(FeatureDataInfo[] data) => _fragmentStore = data.Select(i => new Data(i.Type, i.DefaultValue)).ToArray();

		public T GetData<T>() where T : FeatureData => (T)_fragmentStore.First(s => typeof(T).IsAssignableTo(s.Type)).Object;
		public void RefreshData<T>() where T : FeatureData => _parentEventHandler?.OnDataChange(GetData<T>());
		public void SetData(FeatureData newValue)
		{
			var index = GetFragmentIndex(newValue);
			_fragmentStore[index].Object = newValue;
			_parentEventHandler?.OnDataChange(newValue);
		}

		public void SetDataChangeHandler(IFeatureDataChangeEventHandler? eventHandler) => _parentEventHandler = eventHandler;

		private int GetFragmentIndex(FeatureData data)
		{
			Type targetType = data.GetType();
			int index = Array.FindIndex(_fragmentStore, s => targetType.IsAssignableTo(s.Type));
			if (index == -1) throw new Exception("Attempt to set unregistered fragment!");
			return index;
		}

		public struct Data
		{
			public Type Type;
			public FeatureData Object;

			public Data(Type type, FeatureData obj) => (Type, Object) = (type, obj);
		}
	}
}