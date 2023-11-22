namespace ABCo.Multicam.Server.Features
{
	public readonly struct FeatureReference
	{
		readonly IFeature _feature;
		public FeatureReference(IFeature feature) => _feature = feature;
		//public readonly IFeature? TryGetFeature() => _feature.IsActive ? _feature : null;
	}
}
