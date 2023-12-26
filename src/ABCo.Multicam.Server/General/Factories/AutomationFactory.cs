using ABCo.Multicam.Server.Automation.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General.Factories
{
	public interface IAutomationFactory
	{
		IScriptButton CreateAutomationButton();
	}

	public class AutomationFactory : IAutomationFactory
	{
		public IScriptButton CreateAutomationButton() => new ScriptButton();
	}
}
