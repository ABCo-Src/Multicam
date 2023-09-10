using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.Features.Data;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IFeatureContentFactory
	{
		ILiveFeature GetLiveFeature(FeatureTypes type, ILocalFragmentCollection collection);
		FeatureDataInfo[] GetFeatureFragments(FeatureTypes type);
		IFeaturePresenter? GetFeaturePresenter(FeatureTypes type, IFeature feature);
	}

	public class FeatureContentFactory : IFeatureContentFactory
	{
		IServiceSource _servSource;
		public FeatureContentFactory(IServiceSource servSource) => _servSource = servSource;

		public FeatureDataInfo[] GetFeatureFragments(FeatureTypes type) => type switch
		{
			FeatureTypes.Switcher => SwitcherDataSpecs.DataInfo,
			_ => new FeatureDataInfo[]
			{
				new FeatureDataInfo(typeof(FeatureGeneralInfo), new FeatureGeneralInfo(FeatureTypes.Unsupported, "New Unknown"))
			}
		};

		public ILiveFeature GetLiveFeature(FeatureTypes type, ILocalFragmentCollection collection) => type switch
		{
			FeatureTypes.Switcher => _servSource.Get<ISwitcherLiveFeature, ILocalFragmentCollection>(collection),
			_ => _servSource.Get<IUnsupportedLiveFeature, ILocalFragmentCollection>(collection)
		};

		public IFeaturePresenter? GetFeaturePresenter(FeatureTypes type, IFeature feature) => type switch
		{
			FeatureTypes.Switcher => _servSource.Get<ISwitcherFeaturePresenter, IFeature>(feature),
			_ => null
		};
	}
}
