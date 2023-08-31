using ABCo.Multicam.UI.Blazor.Views.Menus;
using ABCo.Multicam.UI.Services;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class UIDialogHandler : IUIDialogHandler
	{
		public static MenuHandler? System { get; set; }

		public void OpenContextMenu(ContextMenuDetails details)
		{
			if (System == null) throw new Exception("Menu handler not initialized!");
			System.ContextMenu = details;
		}
	}
}
