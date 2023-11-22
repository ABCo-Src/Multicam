using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server
{
	public interface IBindableServerComponent<T> : IServerService<T>, IDisposable
	{
		IClientNotifier<T> ClientNotifier { get; }
	}

	public abstract class BindableServerComponent<T> : ObservableObject, IBindableServerComponent<T>
	{
		public IClientNotifier<T> ClientNotifier { get; }

		public BindableServerComponent(IServerInfo info)
		{
			var asComponent = (T)(object)this;
			ClientNotifier = info.ClientsManager.NewClientsDataNotifier(asComponent);

			// Set this up to notify clients
			PropertyChanged += (s, e) => ClientNotifier.Notify(e.PropertyName);
		}

		public void Dispose()
		{
			ClientNotifier.Dispose();
			DisposeComponent();
		}

		public abstract void DisposeComponent();
	}
}
