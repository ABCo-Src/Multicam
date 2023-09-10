namespace ABCo.Multicam.Core.Features.Data
{
	public struct FeatureDataInfo
	{
		public Type Type;
		public FeatureData DefaultValue;

		public FeatureDataInfo(Type type, FeatureData defaultValue) => (Type, DefaultValue) = (type, defaultValue);
	}
}
