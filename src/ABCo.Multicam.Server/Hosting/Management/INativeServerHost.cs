using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management.Data;

namespace ABCo.Multicam.Server.Hosting.Management
{
    public record struct NativeServerHostConfig(string Host);

	public interface INativeServerHost : IServerService<NativeServerHostConfig>, IAsyncDisposable
	{
		Task Start();
		Task Stop();
	}
}
