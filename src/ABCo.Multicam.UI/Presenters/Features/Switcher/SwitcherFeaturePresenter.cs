using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher
{
	public interface ISwitcherFeaturePresenter : ISpecificFeaturePresenter
	{

	}

	public class SwitcherFeaturePresenter : ISwitcherFeaturePresenter
	{
		IServiceSource _servSource;

		IFeature _baseFeature = null!;
		ISwitcherFeatureVM _vm;
		ISwitcherMixBlocksPresenter _mixBlocksPresenter = null!;
		ISwitcherMixBlocksPresenter _connectionPresenter = null!;

		public SwitcherFeaturePresenter(IServiceSource servSource)
		{
			_servSource = servSource;
			_vm = servSource.Get<ISwitcherFeatureVM>();
		}

		public void FinishConstruction(IFeature baseFeature)
		{
			_baseFeature = baseFeature;
			_mixBlocksPresenter = _servSource.Get<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IFeature>(_vm, _baseFeature);
		}

		public void OnFragmentUpdate<T>(int code, T structure)
		{
			var id = (SwitcherFeatureFragmentID)code;
			switch (id)
			{
				case SwitcherFeatureFragmentID.SwitcherConfig:
					_vm.Config = _servSource.Get<ISwitcherConfigVM, SwitcherConfig, ISwitcherFeatureVM>((SwitcherConfig)(object)structure!, _vm);
					break;
				case SwitcherFeatureFragmentID.SwitcherSpecs:
					_mixBlocksPresenter.UpdateSpecs((SwitcherSpecs)(object)structure!);
					break;
				case SwitcherFeatureFragmentID.SwitcherState:
					_mixBlocksPresenter.UpdateState((MixBlockState[])(object)structure!);
					break;
			}
		}
	}
}
