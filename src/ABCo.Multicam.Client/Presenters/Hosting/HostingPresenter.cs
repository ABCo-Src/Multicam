using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Hosting.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.Presenters.Hosting
{
	public class HostingPresenter : IClientDataNotificationTarget, IClientService<IServerTarget>
	{
		readonly IServerTarget _target;

		public HostingPresenter(IServerTarget target)
		{
			_target = target;
		}

		public void Init()
		{
			
		}

		public void OnDataChange(ServerData obj)
		{
			throw new NotImplementedException();
		}
	}
}
