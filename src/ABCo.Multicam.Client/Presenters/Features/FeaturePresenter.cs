using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Client.Presenters.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features
{

	public interface IMainFeaturePresenter : IClientDataNotificationTarget
	{
		IFeatureVM VM { get; }
		void OnTitleChange();
	}

	public interface IFeatureContentPresenter : IClientDataNotificationTarget
	{
		IFeatureContentVM VM { get; }
	}

	public class FeaturePresenter : IMainFeaturePresenter
	{
		public IFeatureVM VM { get; private set; }

		readonly IClientInfo _info;
		readonly IServerTarget _feature;
		FeatureTypes? _type;

		public FeaturePresenter(IServerTarget feature, IClientInfo info)
		{
			_info = info;
            _feature = feature;

			VM = info.Get<IFeatureVM, IMainFeaturePresenter>(this);
        }

		public void Init() { }

        public void OnDataChange(ServerData structure)
		{
			if (structure is FeatureGeneralInfo info)
			{
				// Update the content presenter
				_type = info.Type;

                var newContentPresenter = _type switch
				{
					FeatureTypes.Switcher => _feature.DataStore.GetOrAddClientEndpoint<ISwitcherFeaturePresenter>(_info),
					_ => null
				};

                if (newContentPresenter != null) VM.Content = newContentPresenter.VM;

				// Update the title
				VM.FeatureTitle = info.Title;
			}
		}

		public void OnTitleChange() => _feature.PerformAction(0, new FeatureGeneralInfo(_type ?? throw new Exception("Uninitialized feature presenter asked to change title."), VM.FeatureTitle));
	}
}
