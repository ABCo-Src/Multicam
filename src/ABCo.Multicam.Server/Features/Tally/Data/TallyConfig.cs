using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features.Tally.Data
{
	public class TallyConfig : ServerData
	{
		public FeatureReference SourceSwitcher { get; }
		public TallyConfig(FeatureReference sourceSwitcher) => SourceSwitcher = sourceSwitcher;
	}
}
