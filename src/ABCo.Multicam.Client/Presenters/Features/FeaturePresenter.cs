using ABCo.Multicam.Server;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Client.Presenters.Features.Switchers;

namespace ABCo.Multicam.Client.Presenters.Features
{

    public interface IFeaturePresenter : IClientDataNotificationTarget<IFeature>
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
		readonly Dispatched<IFeature> _feature;
		readonly IFeatureContentPresenter _contentPresenter;

		public FeaturePresenter(Dispatched<IFeature> feature, IClientInfo info)
		{
			_info = info;
            _feature = feature;

			VM = info.Get<IFeatureVM, IFeaturePresenter>(this);

			_contentPresenter = _feature.Get(f => f.Type) switch
			{
				FeatureTypes.Switcher => info.Get<ISwitcherFeaturePresenter, Dispatched<IFeature>>(feature),
				_ => throw new Exception("Unsupported!")
			};

			VM.Content = _contentPresenter.VM;
		}

		public void Init() { }

		public void OnServerStateChange(string? changedProp)
		{
			// Update the title
			VM.FeatureTitle = _feature.Get(f => f.Name);

			// Inform the content presenter of the change
			switch (_feature.Get(f => f.Type))
			{
				case FeatureTypes.Switcher:
					((ISwitcherFeaturePresenter)_contentPresenter).OnServerStateChange(changedProp);
					break;
			}
		}

		public void OnTitleChange() => _feature.CallDispatched(f => f.Rename(VM.FeatureTitle));
	}
}
