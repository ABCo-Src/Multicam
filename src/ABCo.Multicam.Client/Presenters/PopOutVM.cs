using ABCo.Multicam.Client.Services;
using ABCo.Multicam.Client.Structures;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IPopOutContentVM
	{
		string Title { get; }
	}

	public record struct ContextMenuDetails(string Title, Action<string> OnSelect, Action? OnCancel, CursorPosition Pos, string[] Items);

	public interface IPopOutVM : INotifyPropertyChanged
	{
		// Context Menu:
		ContextMenuDetails? ContextMenu { get; }
		void ContextChooseItem(string item);

		// Custom Content:
		IPopOutContentVM? CustomContent { get; }
		void Open(IPopOutContentVM vm);
		void OpenContext(ContextMenuDetails? contextMenu);

		// General:
		bool ShowMenu { get; }
		void Close();
		void CloseIfOfType<T>();
	}

	public partial class PopOutVM : ViewModelBase, IPopOutVM
	{
		[ObservableProperty] ContextMenuDetails? _contextMenu;
		[ObservableProperty] IPopOutContentVM? _customContent;

		public bool ShowMenu => ContextMenu != null || CustomContent != null;

		public void Open(IPopOutContentVM vm)
		{
			Close();
			CustomContent = vm;
		}

		public void OpenContext(ContextMenuDetails? contextMenu)
		{
			Close();
			ContextMenu = contextMenu;
		}

		public void ContextChooseItem(string item)
		{
			ContextMenu!.Value.OnSelect(item);
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
	}
}
