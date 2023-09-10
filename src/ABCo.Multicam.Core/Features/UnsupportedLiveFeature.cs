using ABCo.Multicam.Core.Features.Data;

namespace ABCo.Multicam.Core.Features
{
	public interface IUnsupportedLiveFeature : ILiveFeature, IParameteredService<ILocalFragmentCollection> { }
    public class UnsupportedLiveFeature : IUnsupportedLiveFeature
    {
		ILocalFragmentCollection _collection;

		public static IUnsupportedLiveFeature New(ILocalFragmentCollection collection) => new UnsupportedLiveFeature(collection);
		public UnsupportedLiveFeature(ILocalFragmentCollection collection) => _collection = collection;

		public void Dispose() { }

		public void PerformAction(int id) { }
		public void PerformAction(int code, object? param)
		{
			if (code == 0)
				_collection.SetData((FeatureGeneralInfo)param!);
		}
	}
}
