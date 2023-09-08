namespace ABCo.Multicam.Core.Features
{
	public interface ILiveFeatureBinder { }
    public interface ILiveFeature : IDisposable
    {
        ILiveFeatureBinder UIBinder { get; }
        FeatureTypes FeatureType { get; }
    }
}
