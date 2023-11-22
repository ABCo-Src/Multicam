using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Features;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public interface ISwitcherFeaturePresenter : IFeatureContentPresenter, IClientDataNotificationTarget<IFeature> { }
	public class SwitcherFeaturePresenter : ISwitcherFeaturePresenter
	{
		readonly IClientInfo _info;

		readonly Dispatched<ISwitcherFeature> _feature;
		readonly ISwitcherFeatureVM _vm;
		readonly ISwitcherConnectionPresenter _connectionPresenter;
		readonly ISwitcherMixBlocksPresenter _mixBlocksPresenter;
		readonly ISwitcherConfigPresenter _configPresenter;

		public SwitcherFeaturePresenter(Dispatched<IFeature> feature, IClientInfo info)
		{
			_info = info;
			_vm = info.Get<ISwitcherFeatureVM>();

			_feature = feature.CastTo<ISwitcherFeature>();

			_connectionPresenter = _info.Get<ISwitcherConnectionPresenter, Dispatched<ISwitcherFeature>>(_feature);
			_vm.Connection = _connectionPresenter.VM;

			_mixBlocksPresenter = _info.Get<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, Dispatched<ISwitcherFeature>>(_vm, _feature);
			_configPresenter = _info.Get<ISwitcherConfigPresenter, Dispatched<ISwitcherFeature>>(_feature);

			OnServerStateChange(null); // Refresh everything
		}

		public IFeatureContentVM VM => _vm;

		public void OnServerStateChange(string? changedProp)
		{
			// Refresh config
			_configPresenter.Refresh(_feature.Get(f => f.Config), _feature.Get(f => f.PlatformCompatibility));

			// Refresh mix blocks/state
			_mixBlocksPresenter.Refresh(_feature.Get(f => f.SpecsInfo));

			// Inform the connection presenter things has changed - only if they have though, otherwise its "in-progress" indicator gets confused.
			// (TODO: Redo that entire thing... A good idea would be to merge "IsConnected" and "ErrorMessage" into one atomic thing for best results.)
			if (changedProp is null or nameof(ISwitcherFeature.IsConnected))
				_connectionPresenter.OnConnection(_feature.Get(f => f.IsConnected));
			if (changedProp is null or nameof(ISwitcherFeature.ErrorMessage))
				_connectionPresenter.OnError(_feature.Get(f => f.ErrorMessage));
			if (changedProp is null or nameof(ISwitcherFeature.SpecsInfo))
				_connectionPresenter.OnSpecced(_feature.Get(f => f.SpecsInfo).Specs);

			_connectionPresenter.OnConnection(_feature.Get(f => f.IsConnected));
		}

		public void Init()
		{
		}
	}
}
