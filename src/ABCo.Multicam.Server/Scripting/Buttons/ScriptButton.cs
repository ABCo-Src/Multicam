using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Scripting.Console;
using ABCo.Multicam.Server.Scripting.Execution;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Automation.Buttons
{
	public interface IScriptButton : INamedServerComponent, IBindableServerComponent<IScriptButton>, IDisposable { }

	public partial class ScriptButton : BindableServerComponent<IScriptButton>, IScriptButton, IScriptID
	{
		[ObservableProperty] string _name = "New Automation Button";
		[ObservableProperty] IEditableScript _script;

		public ScriptButton(IServerInfo info) => Script = new EditableScript(this, info);

		public string GetID() => "Button, " + Name;
		public void Rename(string name) => Name = name;

		public void Dispose() { }
	}
}
