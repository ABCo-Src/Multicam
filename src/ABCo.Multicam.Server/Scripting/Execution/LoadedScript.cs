using ABCo.Multicam.Server.Scripting.Console;
using ABCo.Multicam.Server.Scripting.Proxy;
using ABCo.Multicam.Server.Scripting.Proxy.Features.Switchers;
using CommunityToolkit.Mvvm.ComponentModel;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Execution
{
	public interface ILoadedScript : INotifyPropertyChanged
	{
		bool IsRunning { get; }
		void Start();
		void Stop();
	}

	public interface IRunningScript
	{
		bool ContinueExecution();
	} 

	public partial class LoadedScript : BindableServerComponent<ILoadedScript>, ILoadedScript, IRunningScript
	{
		readonly IScriptID _id;
		readonly IScriptManager _manager;
		readonly IScriptExecutionManager _executionManager;
		readonly Script _script;
		readonly DynValue _loadedCode;
		Coroutine? _executionCoroutine;

		[ObservableProperty] bool _isRunning;

		int _startRequestCount;
		bool _stopRequest;

		public LoadedScript(string code, IScriptID id, IServerInfo info)
		{
			_id = id;
			_manager = info.Shared.ScriptManager;
			_executionManager = info.Shared.ScriptManager.Execution;

			// Create the script, adding global variables
			_script = new Script(CoreModules.Preset_HardSandbox);
			_script.Globals["print"] = (Action<string>)((s) => _manager.Console.WriteLine(s, id, ConsoleMessageType.Print));
			_script.Globals["input"] = (Action)(() => throw new Exception("Script input() is not currently supported in the Multicam Platform."));
			_script.Globals["sleep"] = (Action<int>)(t =>
			{
				Thread.Sleep(t);
				info.Dispatcher.Yield();
			});
			_script.Globals["Switchers"] = new SwitchersGlobalProxy(this, info);

			_loadedCode = _script.LoadString(code);
		}

		public void Start()
		{
			// Just queue if we're currently running.
			if (IsRunning)
			{
				_startRequestCount++;
				return;
			}

			// Otherwise, set this up on the execution manager.
			IsRunning = true;
			_executionManager.Execute(this);
		}

		public void Stop() => _stopRequest = true;

		public bool ContinueExecution()
		{
			// Do nothing if we're not running.
			if (!IsRunning) return false;

			// Create a new co-routine if there isn't one present.
			if (_executionCoroutine == null)
			{
				_executionCoroutine = _script.CreateCoroutine(_loadedCode).Coroutine;
				_executionCoroutine.AutoYieldCounter = 100; // So stuck scripts still yield back to the server for stopping
			}

			// Resume, and stop if needed.
			try
			{
				if (_stopRequest || _executionCoroutine.Resume().Type != DataType.YieldRequest) 
					return HandleStopRequest();
			}
			catch (Exception ex)
			{
				_manager.HandleScriptError(_id, ex);
				return HandleStopRequest();
			}

			return true;

			bool HandleStopRequest()
			{
				_stopRequest = false;
				_executionCoroutine = null;

				if (_startRequestCount == 0) IsRunning = false;
				else _startRequestCount--;

				return IsRunning;
			}
		}
	}
}
