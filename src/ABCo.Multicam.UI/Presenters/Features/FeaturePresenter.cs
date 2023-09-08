using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{
    public interface IFeaturePresenter : IGeneralFeaturePresenter
    {

    }

	public class FeaturePresenter : IFeaturePresenter
	{
		IFeature _feature;
		IFeatureVM _featureVM;
		bool _isEditing;

		public void FinishConstruction(IFeature param2, IFeatureVM targetVM)
		{
			_featureVM = targetVM;
			_feature = param2;
		}

		public void ToggleEdit()
		{
			
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
