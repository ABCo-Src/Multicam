using ABCo.Multicam.Client.Blazor.Views.Menus;
using ABCo.Multicam.Client.Services;

namespace ABCo.Multicam.Client.Blazor.Services
{
	public class UIDialogHandler : IUIDialogHandler
	{
		MenuHandler? _system;

		public void Associate(MenuHandler system) => _system = system;
		public void OpenContextMenu(ContextMenuDetails details)
		{
			if (_system == null) throw new Exception("Menu handler not initialized!");
			_system.ContextMenu = details;
		}
	}
}
