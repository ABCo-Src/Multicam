using ABCo.Multicam.Core.Features.Data;

namespace ABCo.Multicam.Core.Features
{
	public interface IReadOnlyFeatureDataSource
	{
		void SetDataChangeHandler(IFeatureDataChangeEventHandler? eventHandler);
		void RefreshData<T>() where T : FeatureData;
	}

	public interface IFeatureDataSource : IReadOnlyFeatureDataSource
	{
		void SetData(FeatureData val);
	}

	public interface IInstantRetrievalDataSource : IFeatureDataSource
	{
		T GetData<T>() where T : FeatureData;
	}

	public interface IFeatureDataChangeEventHandler
	{
		void OnDataChange(FeatureData val);
	}
}
