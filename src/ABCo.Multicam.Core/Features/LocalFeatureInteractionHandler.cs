using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features;
using BMDSwitcherAPI;
using System.Diagnostics;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ABCo.Multicam.Core.Features
{
    // Interaction handler for a locally-running feature
	public interface ILocalFeatureInteractionHandler : IFeatureInteractionHandler, IParameteredService<FeatureTypes, FeatureDataInfo[]> { }
	public class LocalFeatureInteractionHandler : ILocalFeatureInteractionHandler, ILocalFragmentCollection
	{
        readonly FeatureData[] _fragmentStore;
        readonly ILiveFeature _runningFeature;
		IFragmentChangeEventHandler? _parentEventHandler;

		public static ILocalFeatureInteractionHandler New(FeatureTypes type, FeatureDataInfo[] fragments, IServiceSource servSource) 
			=> new LocalFeatureInteractionHandler(type, fragments, servSource);
        public LocalFeatureInteractionHandler(FeatureTypes type, FeatureDataInfo[] fragments, IServiceSource servSource)
        {
			_fragmentStore = fragments.Select(i => i.DefaultValue).ToArray();
			_runningFeature = servSource.Get<IFeatureContentFactory>().GetLiveFeature(type, this);
		}

		public T GetData<T>() where T : FeatureData => (T)_fragmentStore.First(s => s is T);
		public void RefreshData<T>() where T : FeatureData => _parentEventHandler?.OnDataChange(GetData<T>());
		public void SetData(FeatureData newValue)
		{
			var index = GetFragmentIndex(newValue);
			_fragmentStore[index] = newValue;
			_parentEventHandler?.OnDataChange(newValue);
		}

		public void PerformAction(int id) => _runningFeature.PerformAction(id);
		public void PerformAction(int id, object structure) => _runningFeature.PerformAction(id, structure);
		public void SetFragmentChangeHandler(IFragmentChangeEventHandler? eventHandler) => _parentEventHandler = eventHandler;

		private int GetFragmentIndex(FeatureData data)
		{
			Type targetType = data.GetType();
			int index = Array.FindIndex(_fragmentStore, s => s.GetType() == targetType);
			if (index == -1) throw new Exception("Attempt to set unregistered fragment!");
			return index;
		}

		public void Dispose() => _runningFeature.Dispose();
	}
}