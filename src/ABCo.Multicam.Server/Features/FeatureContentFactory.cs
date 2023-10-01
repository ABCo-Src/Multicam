﻿using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Data;

namespace ABCo.Multicam.Client.ViewModels.Features
{
	public interface IFeatureContentFactory
	{
		ILiveFeature GetLiveFeature(FeatureTypes type, IInstantRetrievalDataSource collection);
		FeatureDataInfo[] GetFeatureDataEntries(FeatureTypes type);
	}

	public class FeatureContentFactory : IFeatureContentFactory
	{
		IServerInfo _servSource;
		public FeatureContentFactory(IServerInfo servSource) => _servSource = servSource;

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
    }
}
