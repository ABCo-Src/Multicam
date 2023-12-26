using ABCo.Multicam.Server.General;
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

	public partial class ScriptButton : BindableServerComponent<IScriptButton>, IScriptButton
	{
		[ObservableProperty] string _name = "New Automation Button";
		public void Rename(string name) => Name = name;

		public void Dispose() { }
	}
}
