using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Features;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public interface ISwitcherFeaturePresenter : IFeatureContentPresenter, IClientDataNotificationTarget<IFeatureState, IFeature> { }
	public class SwitcherFeaturePresenter : ISwitcherFeaturePresenter
	{
		readonly IClientInfo _info;

		readonly IDispatchedServerComponent<ISwitcherFeature> _feature;
		readonly ISwitcherFeatureState _state;
		readonly ISwitcherFeatureVM _vm;
		readonly ISwitcherConnectionPresenter _connectionPresenter;
		readonly ISwitcherMixBlocksPresenter _mixBlocksPresenter;
		readonly ISwitcherConfigPresenter _configPresenter;

		public SwitcherFeaturePresenter(IFeatureState state, IDispatchedServerComponent<IFeature> feature, IClientInfo info)
		{
			_info = info;
			_vm = info.Get<ISwitcherFeatureVM>();

			_state = (ISwitcherFeatureState)state;
			_feature = feature.CastTo<ISwitcherFeature>();

			OnServerStateChange(null); // Refresh everything

			_connectionPresenter = _info.Get<ISwitcherConnectionPresenter, IDispatchedServerComponent<ISwitcherFeature>>(_feature);
			_vm.Connection = _connectionPresenter.VM;

			_mixBlocksPresenter = _info.Get<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IDispatchedServerComponent<ISwitcherFeature>>(_vm, _feature);
			_configPresenter = _info.Get<ISwitcherConfigPresenter, IDispatchedServerComponent<ISwitcherFeature>>(_feature);
		}

		public IFeatureContentVM VM => _vm;

		public void OnServerStateChange(string? changedProp)
		{
			// Refresh config
			_configPresenter.Refresh(_state.Config, _state.PlatformCompatibility);

			// Refresh mix blocks/state
			_mixBlocksPresenter.Refresh(_state.SpecsInfo);

			// Inform the connection presenter things has changed - only if they have though, otherwise its "in-progress" indicator gets confused.
			// (TODO: Redo that entire thing... A good idea would be to merge "IsConnected" and "ErrorMessage" into one atomic thing for best results.)
			if (changedProp is null or nameof(ISwitcherFeatureState.IsConnected))
				_connectionPresenter.OnConnection(_state.IsConnected);
			if (changedProp is null or nameof(ISwitcherFeatureState.ErrorMessage))
				_connectionPresenter.OnError(_state.ErrorMessage);
			if (changedProp is null or nameof(ISwitcherFeatureState.SpecsInfo))
				_connectionPresenter.OnSpecced(_state.SpecsInfo.Specs);

			_connectionPresenter.OnConnection(_state.IsConnected);
		}

		public void Init()
		{
		}
	}
}
