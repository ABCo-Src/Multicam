using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features.Data
{
	public class FeaturesList : ServerData
	{
		public IList<IServerTarget> Features { get; }
		public FeaturesList(IList<IServerTarget> features) => Features = features;
	}
}
