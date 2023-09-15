using ABCo.Multicam.Core.Features.Data;

namespace ABCo.Multicam.Core.Features
{
	public interface IUnsupportedLiveFeature : ILiveFeature, IParameteredService<IInstantRetrievalDataSource> { }
    public class UnsupportedLiveFeature : IUnsupportedLiveFeature
    {
		IInstantRetrievalDataSource _collection;

		public UnsupportedLiveFeature(IInstantRetrievalDataSource collection) => _collection = collection;

		public void Dispose() { }

		public void PerformAction(int id) { }
		public void PerformAction(int code, object? param)
		{
			if (code == 0)
				_collection.SetData((FeatureGeneralInfo)param!);
		}
	}
}
