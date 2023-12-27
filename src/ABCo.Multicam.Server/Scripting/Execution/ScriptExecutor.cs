using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Execution
{
	public interface IScriptExecutionManager
	{
		void Execute(IRunningScript script);
	}

	/// <summary>
	/// Runs all the registered automations (in a background thread)
	/// </summary>
	public class ScriptExecutionManager : IScriptExecutionManager
	{
		readonly IServerInfo _info;

		List<IRunningScript> _runningScripts = new();
		bool _isRunning = false;

		public ScriptExecutionManager(IServerInfo info)
		{
			_info = info;
		}

		public void Execute(IRunningScript script)
		{
			_runningScripts.Add(script);

			// Start the execution loop
			if (!_isRunning) ExecutionLoop();
		}

		async void ExecutionLoop()
		{
			_isRunning = true;

			while (true)
			{
				// Run every script in the list. They'll yield back here either when waiting for an event, or after a certain number of instructions.
				for (int i = 0; i < _runningScripts.Count; i++)
				{
					if (!_runningScripts[i].ContinueExecution())
					{
						_runningScripts.RemoveAt(i);
						i--;
					}
				}

				if (_runningScripts.Count == 0) break;

				// Yield so we don't hog the server
				await Task.Yield();
			}

			_isRunning = false;
		}
    }
}
