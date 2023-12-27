using ABCo.Multicam.Client.ViewModels.Scripting.Execution;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Scripting.Execution;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Scripting.Editor
{
	public interface IEditableScriptVM : INotifyPropertyChanged
	{
		string Code { get; }
		ILoadedScriptVM? LoadedScript { get; }
		void UpdateCode(string code);
	}

	public partial class EditableScriptVM : BoundViewModelBase<IEditableScript>, IEditableScriptVM
	{
		[ObservableProperty] string _code = "";
		[ObservableProperty] ILoadedScriptVM? _loadedScript;

		public EditableScriptVM(Dispatched<IEditableScript> serverComponent, IFrameClientInfo info) : base(serverComponent, info) => OnServerStateChange(null);

		public void UpdateCode(string code) => _serverComponent.CallDispatched(c => c.UpdateCode(code));

		protected override void OnServerStateChange(string? changedProp)
		{
			Code = _serverComponent.Get(s => s.Code);

			if (changedProp == nameof(IEditableScript.LoadedScript) || changedProp == null)
			{
				var loadedScript = _serverComponent.Get(s => s.LoadedScript);
				if (loadedScript != null)
					LoadedScript = new LoadedScriptVM(_info.Server.CreateDispatcher(loadedScript), _info);
			}
		}
	}
}
