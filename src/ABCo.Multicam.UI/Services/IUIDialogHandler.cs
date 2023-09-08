using ABCo.Multicam.UI.Structures;

namespace ABCo.Multicam.UI.Services
{
	public record struct ContextMenuDetails(string Title, Action<string> OnSelect, Action? OnCancel, CursorPosition Pos, string[] Items);

    public interface IUIDialogHandler 
    {
        public void OpenContextMenu(ContextMenuDetails details);
    }
}
