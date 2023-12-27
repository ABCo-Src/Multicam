using ABCo.Multicam.Server.Hosting.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General.Factories
{
	public interface IHostingFactory
	{
		INativeServerHost CreateNativeServerHost(NativeServerHostConfig nativeConfig);
		ILocalIPCollection CreateIPCollection(Action act);
	}

	public class HostingFactory : IHostingFactory
	{
		readonly IServerInfo _info;
		readonly Func<NativeServerHostConfig, IServerInfo, INativeServerHost> _createServerHost;
		readonly Func<Action, ILocalIPCollection> _createIPCollection;

		public HostingFactory(Func<NativeServerHostConfig, IServerInfo, INativeServerHost> createServerHost, Func<Action, ILocalIPCollection> createIPCollection, IServerInfo info)
		{
			_info = info;
			_createServerHost = createServerHost;
			_createIPCollection = createIPCollection;
		}

		public ILocalIPCollection CreateIPCollection(Action act) => _createIPCollection(act);
		public INativeServerHost CreateNativeServerHost(NativeServerHostConfig nativeConfig) => _createServerHost(nativeConfig, _info);
	}
}
