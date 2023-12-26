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
		IAutomationFactory AutomationFactory { get; }
	}

	public class ServerFactories : IServerFactories
	{
		public IAutomationFactory AutomationFactory { get; }

		public ServerFactories()
		{
			AutomationFactory = new AutomationFactory();
		}
	}
}
