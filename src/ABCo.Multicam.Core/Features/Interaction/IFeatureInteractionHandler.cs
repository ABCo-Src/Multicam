using ABCo.Multicam.Core.Features.Data;

namespace ABCo.Multicam.Core.Features
{
	public interface IFeatureInteractionHandler : IDisposable
	{
		void PerformAction(int id);
		void PerformAction(int id, object param);
		void SetFragmentChangeHandler(IFragmentChangeEventHandler? eventHandler);
		void RefreshData<T>() where T : FeatureData;
	}

	public interface ILocalFragmentCollection
	{
		T GetData<T>() where T : FeatureData;
		void SetData(FeatureData val);
	}

	public interface IFragmentChangeEventHandler
	{
		void OnDataChange(FeatureData val);
	}
}
