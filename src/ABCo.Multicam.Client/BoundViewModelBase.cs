using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client;

// GVG Protocol
public class ViewModelBase : ObservableObject { }
public abstract class BoundViewModelBase<T> : ObservableObject, IDisposable where T : INotifyPropertyChanged
{
	protected readonly IFrameClientInfo _info;
	protected Dispatched<T> _serverComponent;

	public BoundViewModelBase(Dispatched<T> serverComponent, IFrameClientInfo info)
	{
		_info = info;
		_serverComponent = serverComponent;

		// TODO: Add some checks to make sure this client doesn't already have a view-model of this type bound to this same server component (because if so we've failed to Dispose something)
		// Dictionary<(object, Type), INotifyPropertyChanged> BoundVMs { get; }

		// Bind to the underlying server - and ensure it gets unbound if the associated client is disconnected
		_serverComponent.CallDispatched(d => d.PropertyChanged += OnServerPropertyChanged);
		info.DisconnectionManager.ClientDisconnected += Dispose;
	}

	protected abstract void OnServerStateChange(string? changedProp);

	void OnServerPropertyChanged(object? src, PropertyChangedEventArgs eventArgs) =>
		_info.Dispatcher.Queue(() => OnServerStateChange(eventArgs.PropertyName)); // (We're on the server's thread, so we move to the client's)

	public virtual void Dispose()
	{
		_info.DisconnectionManager.ClientDisconnected -= Dispose;
		_serverComponent.CallDispatched(d => d.PropertyChanged -= OnServerPropertyChanged);
	}
}