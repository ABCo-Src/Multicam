using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management.Data;

namespace ABCo.Multicam.Server.Hosting.Management
{
    
	public interface INativeServerHost : IServerService<IHostingManager>
    {
		Task Start(HostingConfig config);
		Task Stop();
    }
}
