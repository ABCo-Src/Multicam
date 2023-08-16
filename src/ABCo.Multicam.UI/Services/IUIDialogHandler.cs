using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Services
{
    public enum DialogPosition
    {
        Pointer
    }

    public struct ContextMenuItem<T>
    {
        public string Name;
        public T Value;

        public ContextMenuItem(string name, T value) => (Name, Value) = (name, value);
    }

    public struct ContextMenuDetails<T>
    {
        public string Title;
        public Action<T> OnSelect;
        public Action? OnCancel;
        public ContextMenuItem<T>[] Items;

        public ContextMenuDetails(string title, Action<T> selected, Action? cancel, ContextMenuItem<T>[] items)
            => (Title, OnSelect, OnCancel, Items) = (title, selected, cancel, items);
    }

    public interface IUIDialogHandler 
    {
        public void OpenContextMenu<T>(ContextMenuDetails<T> details);
    }
}
