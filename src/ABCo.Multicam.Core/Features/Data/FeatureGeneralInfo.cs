using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Core.Features.Data
{
	public class FeatureGeneralInfo : ServerData
	{
		public FeatureTypes Type { get; }
		public string Title { get; }

		public FeatureGeneralInfo(FeatureTypes type, string title)
		{
			Type = type;
			Title = title;
		}
	}

	public enum FeatureTypes
	{
		Unsupported,

		// v1 features
		Switcher,
		Tally,
		Logger,
		Generator
	}
}
