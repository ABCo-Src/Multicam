using ABCo.Multicam.Server.General.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General
{
	public interface IServerFactories
	{
		IAutomationFactory Automation { get; }
		ISwitcherFactory Switcher { get; }
		IHostingFactory Hosting { get; }
	}

	public class ServerFactories : IServerFactories
	{
		public IAutomationFactory Automation { get; }
		public ISwitcherFactory Switcher { get; }
		public IHostingFactory Hosting { get; }

		public ServerFactories(IHostingFactory hosting, IServerInfo info)
		{
			Automation = new AutomationFactory();
			Switcher = new SwitcherFactory(info);
			Hosting = hosting;
		}
	}
}
