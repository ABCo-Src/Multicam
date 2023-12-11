using ABCo.Multicam.Client.Structures;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IPopOutContentVM : INotifyPropertyChanged
	{
		string Title { get; }
	}

	public record class ContextMenuDetails(string Title, Action<string> OnSelect, Action? OnCancel, CursorPosition Pos, string[] Items);

	public interface IPopOutVM : INotifyPropertyChanged
	{
		// Context Menu:
		ContextMenuDetails? ContextMenu { get; }
		void ContextChooseItem(string item);

		// Custom Content:
		IPopOutContentVM? CustomContent { get; }
		void Open(IPopOutContentVM vm, CursorPosition pos);
		void OpenContext(ContextMenuDetails contextMenu);

		// General:
		bool ShowMenu { get; }
		double RequestedMenuX { get; }
		double RequestedMenuY { get; }
		void Close();
		void CloseIfOfType<T>();
	}

	public partial class PopOutVM : ViewModelBase, IPopOutVM
	{
		[ObservableProperty] ContextMenuDetails? _contextMenu;
		[ObservableProperty] IPopOutContentVM? _customContent;

		public bool ShowMenu => ContextMenu != null || CustomContent != null;

		public double RequestedMenuX { get; private set; }
		public double RequestedMenuY { get; private set; }

		public void Open(IPopOutContentVM vm)
		{
			Close();
			CustomContent = vm;
		}

		public void OpenContext(ContextMenuDetails contextMenu)
		{
			Close();
			ContextMenu = contextMenu;
			RequestedMenuX = contextMenu.Pos.X;
			RequestedMenuY = contextMenu.Pos.Y;
		}

		public void ContextChooseItem(string item)
		{
			ContextMenu!.OnSelect(item);
			Close();
		}

		public void Close()
		{
			ContextMenu?.OnCancel?.Invoke();
			ContextMenu = null;
			CustomContent = null;
		}

		public void CloseIfOfType<T>()
		{
			if (CustomContent is T) 
				CustomContent = null;
		}

		public void Open(IPopOutContentVM vm, CursorPosition pos)
		{
			CustomContent = vm;
			RequestedMenuX = pos.X;
			RequestedMenuY = pos.Y;
		}
	}
}
