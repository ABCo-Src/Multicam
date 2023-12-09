using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Features;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers
{
    public interface ISwitcherFeaturePresenter : IFeatureContentPresenter, IClientDataNotificationTarget<IFeature> { }
    public class SwitcherFeaturePresenter : ISwitcherFeaturePresenter
    {
        readonly IClientInfo _info;

        readonly Dispatched<ISwitcherFeature> _feature;
        readonly ISwitcherFeatureVM _vm;
        readonly SwitcherConnectionPresenter _connectionPresenter;
        readonly ISwitcherMixBlocksPresenter _mixBlocksPresenter;
        readonly ISwitcherConfigPresenter _configPresenter;

        public SwitcherFeaturePresenter(Dispatched<IFeature> feature, IClientInfo info)
        {
            _info = info;
            _vm = info.Get<ISwitcherFeatureVM>();

            _feature = feature.CastTo<ISwitcherFeature>();

            _connectionPresenter = new SwitcherConnectionPresenter(_feature, info);
            _vm.Connection = _connectionPresenter.VM;

            _mixBlocksPresenter = _info.Get<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, Dispatched<ISwitcherFeature>>(_vm, _feature);

            _configPresenter = _info.Get<ISwitcherConfigPresenter, Dispatched<ISwitcherFeature>>(_feature);
            _vm.Config = _configPresenter.VM;

            OnServerStateChange(null); // Refresh everything
        }

        public IFeatureContentVM VM => _vm;

        public void OnServerStateChange(string? changedProp)
        {
            _configPresenter.Refresh(_feature.Get(f => f.Config), _feature.Get(f => f.PlatformCompatibility));
            _mixBlocksPresenter.Refresh(_feature.Get(f => f.SpecsInfo));
            _connectionPresenter.Refresh();
        }

        public void Init()
        {
        }
    }
}
