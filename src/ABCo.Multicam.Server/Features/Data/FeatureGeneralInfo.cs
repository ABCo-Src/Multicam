using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting;

namespace ABCo.Multicam.Server.Features.Data
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
