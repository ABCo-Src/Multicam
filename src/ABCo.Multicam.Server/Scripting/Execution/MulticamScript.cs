using ABCo.Multicam.Server.Scripting.Console;
using CommunityToolkit.Mvvm.ComponentModel;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Execution
{
    public interface IEditableScript : INotifyPropertyChanged
    {
        string Code { get; }
        string? CurrentCompilationError { get; }
        ILoadedScript? LoadedScript { get; }
        
        void UpdateCode(string code);
    }

    public partial class EditableScript : BindableServerComponent<IEditableScript>, IEditableScript
	{
        readonly IScriptID _id;
        readonly IServerInfo _info;
        RunningScriptState _executionState;

        [ObservableProperty] string _code = "";
        [ObservableProperty] string? _currentCompilationError = null;
        [ObservableProperty] ILoadedScript? _loadedScript;

		public EditableScript(IScriptID id, IServerInfo info)
		{
            _id = id;
			_info = info;
		}

		public void Execute() => LoadedScript?.Start();

		public void UpdateCode(string code)
        {
            Code = code;

            // Stop the script (if it's running)
            if (LoadedScript != null && LoadedScript.IsRunning) LoadedScript.Stop();

            // Attempt to create a new script
            try
            {
				LoadedScript = new LoadedScript(code, _id, _info);
				CurrentCompilationError = null;
			}
            catch (Exception ex)
            {
                CurrentCompilationError = ex.Message;
            }
        }
	}
}
