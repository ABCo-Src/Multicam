using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server
{
	public interface IServerComponentState<TServerComponentState, TServerComponent> : IServerService<TServerComponent>, IDisposable
	{
		IClientNotifier<TServerComponentState, TServerComponent> ClientNotifier { get; }
	}

	public class ServerComponentState<TServerComponentState, TServerComponent> : ObservableObject, IServerComponentState<TServerComponentState, TServerComponent>
	{
		protected TServerComponent _component;
		public IClientNotifier<TServerComponentState, TServerComponent> ClientNotifier { get; }

		public ServerComponentState(TServerComponent component, IServerInfo info)
		{
			_component = component;
			var asState = (TServerComponentState)(object)this;
			ClientNotifier = info.ClientsManager.NewClientsDataNotifier(asState, component);

			// Set this up to notify clients
			PropertyChanged += (s, e) => ClientNotifier.Notify(e.PropertyName);
		}

		public void Dispose() => ClientNotifier.Dispose();
	}
}
