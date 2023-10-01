using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting;

namespace ABCo.Multicam.Server.Features
{
    public interface IReadOnlyFeatureDataSource
	{
		void SetDataChangeHandler(IFeatureDataChangeEventHandler? eventHandler);
		void RefreshData<T>() where T : ServerData;
	}

	public interface IFeatureDataSource : IReadOnlyFeatureDataSource
	{
		void SetData(ServerData val);
	}

	public interface IInstantRetrievalDataSource : IFeatureDataSource
	{
		T GetData<T>();
	}

	public interface IFeatureDataChangeEventHandler
	{
		void OnDataChange(ServerData val);
	}
}
