using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Features;

namespace ABCo.Multicam.Client.Presenters.Features
{

	public interface IFeaturePresenter : IClientDataNotificationTarget<IFeatureState, IFeature>
	{
		IFeatureVM VM { get; }
		void OnTitleChange();
	}

	public interface IFeatureContentPresenter
	{
		IFeatureContentVM VM { get; }
	}

	public class FeaturePresenter : IFeaturePresenter
	{
		public IFeatureVM VM { get; private set; }

		readonly IClientInfo _info;
		readonly IDispatchedServerComponent<IFeature> _feature;
		readonly IFeatureState _state;
		readonly IFeatureContentPresenter _contentPresenter;

		public FeaturePresenter(IFeatureState state, IDispatchedServerComponent<IFeature> feature, IClientInfo info)
		{
			_info = info;
            _feature = feature;

			VM = info.Get<IFeatureVM, IFeaturePresenter>(this);

			_contentPresenter = state.Type switch
			{
				FeatureTypes.Switcher => state.ClientNotifier.GetOrAddClientEndpoint<ISwitcherFeaturePresenter>(_info),
				_ => null
			};
		}

		public void Init() { }

		public void OnServerStateChange(string? changedProp)
		{
			// Update the title
			VM.FeatureTitle = _state.Name;

			// Inform the content presenter of the change
			switch (_state.Type)
			{
				case FeatureTypes.Switcher:
					((ISwitcherFeaturePresenter)_contentPresenter).OnServerStateChange(changedProp);
					break;
			}
		}

		public void OnTitleChange() => _feature.CallDispatched(f => f.Rename(VM.FeatureTitle));
	}
}
