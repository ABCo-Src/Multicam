using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Core.Features
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
