using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Hosting;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
    public interface ISwitcherFeaturePresenter : IFeatureContentPresenter { }
	public class SwitcherFeaturePresenter : ISwitcherFeaturePresenter
	{
		readonly IClientInfo _info;

		readonly IServerTarget _feature;
		readonly ISwitcherFeatureVM _vm;
		readonly ISwitcherConnectionPresenter _connectionPresenter;
		readonly ISwitcherMixBlocksPresenter _mixBlocksPresenter;
		ISwitcherConfigPresenter? _configPresenter;

		public SwitcherFeaturePresenter(IServerTarget baseFeature, IClientInfo info)
		{
			_info = info;
			_vm = info.Get<ISwitcherFeatureVM, IServerTarget>(baseFeature);

			_feature = baseFeature;

			_connectionPresenter = _info.Get<ISwitcherConnectionPresenter, IServerTarget>(_feature);
			_vm.Connection = _connectionPresenter.VM;

			_mixBlocksPresenter = _info.Get<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IServerTarget>(_vm, _feature);
		}

		public IFeatureContentVM VM => _vm;

		public void OnDataChange(ServerData data)
		{
			switch (data)
			{
				case SwitcherConfigType configType:
					_configPresenter = _info.Get<ISwitcherConfigPresenter, IServerTarget, SwitcherConfigType>(_feature, configType);
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

				case SwitcherCompatibility compatibility:
					_configPresenter?.OnCompatibility(compatibility);
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
