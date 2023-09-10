using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers.Data;

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
			FeatureTypes.Switcher => new FeatureDataInfo[]
			{
				new FeatureDataInfo(typeof(FeatureGeneralInfo), new FeatureGeneralInfo(FeatureTypes.Switcher, "New Switcher")),
				new FeatureDataInfo(typeof(SwitcherConfig), new DummySwitcherConfig(4)),
				new FeatureDataInfo(typeof(SwitcherSpecs), new SwitcherSpecs()),
				new FeatureDataInfo(typeof(SwitcherState), new SwitcherState(Array.Empty<MixBlockState>())),
				new FeatureDataInfo(typeof(SwitcherConnection), new SwitcherConnection(false)),
				new FeatureDataInfo(typeof(SwitcherError), new SwitcherError(null))
			},
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
