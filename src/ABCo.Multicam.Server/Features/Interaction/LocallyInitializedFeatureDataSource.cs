using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.Server.Features
{
    public interface ILocallyInitializedFeatureDataSource : IFeatureDataSource, IInstantRetrievalDataSource, IServerService<FeatureDataInfo[]> { }
	public class LocallyInitializedFeatureDataSource : ILocallyInitializedFeatureDataSource
	{
        readonly Data[] _fragmentStore;
		IFeatureDataChangeEventHandler? _parentEventHandler;

		public LocallyInitializedFeatureDataSource(FeatureDataInfo[] data) => _fragmentStore = data.Select(i => new Data(i.Type, i.DefaultValue)).ToArray();

		public T GetData<T>() => (T)_fragmentStore.First(s => typeof(T).IsAssignableTo(s.Type)).Object;
		public void RefreshData<T>() where T : ServerData => _parentEventHandler?.OnDataChange(GetData<T>()!);
		public void SetData(ServerData newValue)
		{
			var index = GetFragmentIndex(newValue);
			_fragmentStore[index].Object = newValue;
			_parentEventHandler?.OnDataChange(newValue);
		}

		public void SetDataChangeHandler(IFeatureDataChangeEventHandler? eventHandler) => _parentEventHandler = eventHandler;

		private int GetFragmentIndex(object data)
		{
			Type targetType = data.GetType();
			int index = Array.FindIndex(_fragmentStore, s => targetType.IsAssignableTo(s.Type));
			if (index == -1) throw new Exception("Attempt to set unregistered fragment!");
			return index;
		}

		public struct Data
		{
			public Type Type;
			public object Object;

			public Data(Type type, object obj) => (Type, Object) = (type, obj);
		}
	}
}