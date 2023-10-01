using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Core.Features.Data
{
	public struct FeatureDataInfo
	{
		public Type Type;
		public object DefaultValue;

		public FeatureDataInfo(Type type, object defaultValue) => (Type, DefaultValue) = (type, defaultValue);
	}
}
