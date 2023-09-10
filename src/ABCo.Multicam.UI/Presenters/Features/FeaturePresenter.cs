using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{
	public interface IFeaturePresenterForVM : IFeaturePresenter
    {
		IFeatureVM VM { get; }
		void OnTitleChange();
	}

	public interface IFeatureContentPresenterForVM : IFeaturePresenter
	{
		IFeatureContentVM VM { get; }
	}

	public class FeaturePresenter : IFeaturePresenterForVM, IParameteredService<IFeature, FeatureTypes>
	{
		FeatureTypes _type;

		public IFeatureVM VM { get; private set; }

		IFeatureContentPresenterForVM? _contentPresenter;
		readonly IFeature _feature;

		public static IFeaturePresenter New(IFeature feature, FeatureTypes type, IServiceSource servSource) => new FeaturePresenter(feature, type, servSource);
		public FeaturePresenter(IFeature feature, FeatureTypes type, IServiceSource servSource)
		{
			_feature = feature;
			_type = type;
			_contentPresenter = (IFeatureContentPresenterForVM?)servSource.Get<IFeatureContentFactory>().GetFeaturePresenter(type, feature);

			VM = servSource.Get<IFeatureVM, IFeaturePresenterForVM>(this);
			if (_contentPresenter != null) VM.Content = _contentPresenter.VM;
		}

		public void Init()
		{
			_feature.RefreshData<FeatureGeneralInfo>();
			_contentPresenter?.Init();
		}

		public void OnDataChange(FeatureData structure)
		{
			if (structure is FeatureGeneralInfo info)
				VM.FeatureTitle = info.Title;
			else
				_contentPresenter?.OnDataChange(structure);
		}

		public void OnTitleChange() => _feature.InteractionHandler.PerformAction(0, new FeatureGeneralInfo(_type, VM.FeatureTitle));
	}
}
