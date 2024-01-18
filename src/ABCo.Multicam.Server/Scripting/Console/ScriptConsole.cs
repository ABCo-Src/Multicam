using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Console
{
	public enum ConsoleMessageType
	{
		Print,
		Event,
		Error
	}

	public interface IScriptConsole : INotifyPropertyChanged
	{
		ConsoleMessage[] Messages { get; }
		void WriteLine(string message, IScriptID id, ConsoleMessageType type);
	}

	public partial class ScriptConsole : BindableServerComponent<IScriptConsole>, IScriptConsole
	{
		[ObservableProperty] ConsoleMessage[] _messages = Array.Empty<ConsoleMessage>();

		readonly List<ConsoleMessage> _workingList = new();

		public void WriteLine(string message, IScriptID id, ConsoleMessageType type)
		{
			_workingList.Add(new()
			{
				Message = $"{id.GetID()}: {message}",
				Type = type
			});

			Messages = _workingList.ToArray();
		}
	}
}
