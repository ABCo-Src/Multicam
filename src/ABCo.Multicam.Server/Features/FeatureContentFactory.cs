using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Data;

namespace ABCo.Multicam.Client.ViewModels.Features
{
	public interface IFeatureContentFactory
	{
		ILiveFeature GetLiveFeature(FeatureTypes type, IFeatureDataStore collection);
	}

	public class FeatureContentFactory : IFeatureContentFactory
	{
		readonly IServerInfo _servSource;
		public FeatureContentFactory(IServerInfo servSource) => _servSource = servSource;

		public ILiveFeature GetLiveFeature(FeatureTypes type, IFeatureDataStore collection) => type switch
		{
			FeatureTypes.Switcher => _servSource.Get<ISwitcherLiveFeature, IFeatureDataStore>(collection),
			_ => _servSource.Get<IUnsupportedLiveFeature, IFeatureDataStore>(collection)
		};
    }
}
