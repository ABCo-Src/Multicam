using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Hosting.Management
{
	public interface IHostingManagerState : IServerComponentState<IHostingManagerState, IHostingManager>
	{
		bool IsAutomatic { get; internal set; }
		IReadOnlyList<string> CustomModeHostNames { get; internal set; }
		string? ActiveHostName { get; internal set; }
		bool IsConnected { get; internal set; }
	}

	public partial class HostingManagerState : ServerComponentState<IHostingManagerState, IHostingManager>
	{
		[ObservableProperty] bool _isAutomatic = true;
		[ObservableProperty] string? _activeHostName;
		[ObservableProperty] IReadOnlyList<string> _customModeHostNames = new string[] { "http://127.0.0.1:800" };
		[ObservableProperty] bool _isConnected;

		public HostingManagerState(IHostingManager component, IServerInfo info) : base(component, info) { }
	}
}
