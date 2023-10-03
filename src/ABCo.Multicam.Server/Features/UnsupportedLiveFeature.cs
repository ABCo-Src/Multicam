using ABCo.Multicam.Server.Features.Data;

namespace ABCo.Multicam.Server.Features
{
	public interface IUnsupportedLiveFeature : ILiveFeature, IServerService<IFeatureDataStore> { }
    public class UnsupportedLiveFeature : IUnsupportedLiveFeature
    {
		IFeatureDataStore _collection;

		public UnsupportedLiveFeature(IFeatureDataStore collection)
		{
			_collection = collection;
			_collection.SetData<FeatureGeneralInfo>(new FeatureGeneralInfo(FeatureTypes.Unsupported, "New Unsupported Feature"));
		}

		public void Dispose() { }

		public void PerformAction(int id) { }
		public void PerformAction(int code, object? param)
		{
			if (code == 0)
				_collection.SetData<FeatureGeneralInfo>((FeatureGeneralInfo)param!);
		}
	}
}
