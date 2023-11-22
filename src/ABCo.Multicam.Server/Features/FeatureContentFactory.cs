using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server;

namespace ABCo.Multicam.Client.ViewModels.Features
{
	public interface IFeatureContentFactory
	{
		IFeature GetLiveFeature(FeatureTypes type);
	}

	public class FeatureContentFactory : IFeatureContentFactory
	{
		readonly IServerInfo _servSource;
		public FeatureContentFactory(IServerInfo servSource) => _servSource = servSource;

		public IFeature GetLiveFeature(FeatureTypes type) => type switch
		{
			FeatureTypes.Switcher => _servSource.Get<ISwitcherFeature>(),
			_ => _servSource.Get<IUnsupportedLiveFeature>()
		};
    }
}
