using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{
    public interface IFeaturePresenter : IGeneralFeaturePresenter
    {

    }

	public class FeaturePresenter : IFeaturePresenter, INeedsInitialization<IFeature, IFeatureVM>
	{
		IFeature _feature;
		IFeatureVM _featureVM;
		bool _isEditing;

		public void FinishConstruction(IFeature feature, IFeatureVM targetVM)
		{
			_feature = feature;
			_featureVM = targetVM;
		}

		public void OnFragmentUpdate<T>(int code, T structure)
		{
			if (code == (int)FeatureFragmentID.Title)
			{
				_featureVM.FeatureTitle = (string)(object)structure!;
			}
		}
	}
}
