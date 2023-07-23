using System;
using System.Collections.Generic;
using System.Linq;
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
        public T Item;
    }

    public interface IUIDialogHandler 
    {
        public void OpenSimpleContext<T>(ContextMenuItem<T>[] items, Action<T> selected, Action? cancel);
    }
}
