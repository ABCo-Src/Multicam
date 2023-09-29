using ABCo.Multicam.UI.Blazor.Views.Menus;
using ABCo.Multicam.UI.Services;

namespace ABCo.Multicam.UI.Blazor.Services
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
