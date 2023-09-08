namespace ABCo.Multicam.Core.Features
{
	public interface IUnsupportedRunningFeature : ILiveFeature { }
    public interface IBinderForUnsupportedFeature : ILiveFeatureBinder { }

    public class UnsupportedRunningFeature : IUnsupportedRunningFeature
    {
        public ILiveFeatureBinder UIBinder { get; private set; }
        public FeatureTypes FeatureType => FeatureTypes.Unsupported;

        public UnsupportedRunningFeature(IBinderForUnsupportedFeature b) => UIBinder = b;
        public void Dispose() { }
    }
}
