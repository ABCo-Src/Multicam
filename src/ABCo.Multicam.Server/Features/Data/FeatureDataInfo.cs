namespace ABCo.Multicam.Server.Features.Data
{
	public struct FeatureDataInfo
	{
		public Type Type;
		public object DefaultValue;

		public FeatureDataInfo(Type type, object defaultValue) => (Type, DefaultValue) = (type, defaultValue);
	}
}
