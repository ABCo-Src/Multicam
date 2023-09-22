using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher
{
	public interface ISwitcherVMFeaturePresenter : ISwitcherFeaturePresenter, IFeatureContentPresenterForVM { }
	public class SwitcherFeaturePresenter : ISwitcherVMFeaturePresenter
	{
		readonly IServiceSource _servSource;

		readonly IFeature _feature;
		readonly ISwitcherFeatureVM _vm;
		readonly ISwitcherConnectionPresenter _connectionPresenter;
		readonly ISwitcherMixBlocksPresenter _mixBlocksPresenter;
		ISwitcherConfigPresenter? _configPresenter;

		public SwitcherFeaturePresenter(IFeature baseFeature, IServiceSource servSource)
		{
			_servSource = servSource;
			_vm = servSource.Get<ISwitcherFeatureVM, IFeature>(baseFeature);

			_feature = baseFeature;

			_connectionPresenter = _servSource.Get<ISwitcherConnectionPresenter, IFeature>(_feature);
			_vm.Connection = _connectionPresenter.VM;

			_mixBlocksPresenter = _servSource.Get<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IFeature>(_vm, _feature);
		}

		public IFeatureContentVM VM => _vm;

		public void OnDataChange(object data)
		{
			switch (data)
			{
				case SwitcherConfigType configType:
					_configPresenter = _servSource.Get<ISwitcherConfigPresenter, IFeature, SwitcherConfigType>(_feature, configType);
					_vm.Config = _configPresenter.VM;
					break;

				case SwitcherConfig config:
					_configPresenter?.OnConfig(config);
					break;

				case SwitcherConnection connection:
					_connectionPresenter.OnConnection(connection.IsConnected);
					break;

				case SwitcherError error:
					_connectionPresenter.OnError(error.Message);
					break;

				case SwitcherSpecs specs:
					_connectionPresenter.OnSpecced(specs);
					_mixBlocksPresenter.OnSpecced(specs);
					break;

				case SwitcherState state:
					_mixBlocksPresenter.OnState(state.Data);
					break;
			}
		}

		public void Init()
		{
			// Update config, then connection, then specs, then state, then error (order important for the presenter to track what's happening clearly)
			_feature.RefreshData<SwitcherConfigType>();
			_feature.RefreshData<SwitcherConfig>();
			_feature.RefreshData<SwitcherConnection>();
			_feature.RefreshData<SwitcherSpecs>();
			_feature.RefreshData<SwitcherState>();
			_feature.RefreshData<SwitcherError>();
		}
	}
}
