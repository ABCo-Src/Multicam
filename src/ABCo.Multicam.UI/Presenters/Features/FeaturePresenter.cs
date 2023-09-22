using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.UI.ViewModels.Features;

namespace ABCo.Multicam.UI.Presenters.Features
{

	public interface IMainFeaturePresenter : IFeaturePresenter
    {
		IFeatureVM VM { get; }
		void OnTitleChange();
	}

	public interface IFeatureContentPresenter : IFeaturePresenter
	{
		IFeatureContentVM VM { get; }
	}

	public class FeaturePresenter : IMainFeaturePresenter, IParameteredService<IFeature, IScopeInfo>
	{
		public IFeatureVM VM { get; private set; }

		readonly IServiceSource _servSource;
		readonly IScopeInfo _scopeInfo;
		readonly IFeature _feature;
		FeatureTypes? _type;

		public FeaturePresenter(IFeature feature, IScopeInfo scopeInfo, IServiceSource servSource)
		{
			_servSource = servSource;
			_scopeInfo = scopeInfo;
            _feature = feature;

			VM = servSource.Get<IFeatureVM, IMainFeaturePresenter>(this);
        }

        public void Init() => _feature.RefreshData<FeatureGeneralInfo>();

        public void OnDataChange(object structure)
		{
			if (structure is FeatureGeneralInfo info)
			{
				// Update the content presenter
				_type = info.Type;
                var newContentPresenter = (IFeatureContentPresenter?)_servSource.Get<IFeatureContentFactory>().GetRelevantContentPresenterFromStore(info.Type, _feature.UIPresenters, _scopeInfo);
                if (newContentPresenter != null) VM.Content = newContentPresenter.VM;

				// Update the title
				VM.FeatureTitle = info.Title;
			}
		}

		public void OnTitleChange() => _feature.PerformAction(0, new FeatureGeneralInfo(_type ?? throw new Exception("Uninitialized feature presenter asked to change title."), VM.FeatureTitle));
	}
}
