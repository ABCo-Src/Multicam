using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Hosting.Scoping;

namespace ABCo.Multicam.UI.ViewModels.Features
{
	public interface IFeatureContentFactory
	{
		ILiveFeature GetLiveFeature(FeatureTypes type, IInstantRetrievalDataSource collection);
		FeatureDataInfo[] GetFeatureDataEntries(FeatureTypes type);
		IFeaturePresenter? GetRelevantContentPresenterFromStore(FeatureTypes type, IScopedPresenterStore<IFeature> store, IScopeInfo scopeInfo);
	}

	public class FeatureContentFactory : IFeatureContentFactory
	{
		IServiceSource _servSource;
		public FeatureContentFactory(IServiceSource servSource) => _servSource = servSource;

		public FeatureDataInfo[] GetFeatureDataEntries(FeatureTypes type) => type switch
		{
			FeatureTypes.Switcher => SwitcherDataSpecs.DataInfo,
			_ => new FeatureDataInfo[] { new(typeof(FeatureGeneralInfo), new FeatureGeneralInfo(FeatureTypes.Unsupported, "New Unknown")) }
		};

		public ILiveFeature GetLiveFeature(FeatureTypes type, IInstantRetrievalDataSource collection) => type switch
		{
			FeatureTypes.Switcher => _servSource.Get<ISwitcherLiveFeature, IInstantRetrievalDataSource>(collection),
			_ => _servSource.Get<IUnsupportedLiveFeature, IInstantRetrievalDataSource>(collection)
		};

        public IFeaturePresenter? GetRelevantContentPresenterFromStore(FeatureTypes type, IScopedPresenterStore<IFeature> store, IScopeInfo scopeInfo) => type switch
        {
            FeatureTypes.Switcher => store.GetPresenter<ISwitcherFeaturePresenter>(scopeInfo),
            _ => null
        };
    }
}
