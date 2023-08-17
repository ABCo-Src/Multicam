using ABCo.Multicam.UI.Services;
using Avalonia.Controls;

namespace ABCo.Multicam.UI.Avalonia.Services
{
    public class WindowedUIWindow : IUIWindow
    {
        readonly Window _window;
        public WindowedUIWindow(Window window) => _window = window;

        public bool CanMinimize => true;
        public bool CanMaximize => true;
        public bool CloseBtnRecommended => true;
        public bool BorderRecommended => true;
        public void CloseMainWindow() => _window.Close();
        public void RequestMainWindowMaximizeToggle() => _window.WindowState = WindowState.Maximized;
        public void RequestMainWindowMinimize() => _window.WindowState = WindowState.Minimized;
    }

    public class UnwindowedUIWindow : IUIWindow
    {
        public bool CanMinimize => false;
        public bool CanMaximize => false;
        public bool CloseBtnRecommended => false;
        public bool BorderRecommended => false;
        public void CloseMainWindow() { /* TODO: Can't really do anything here... What's the best course of action? */ }
        public void RequestMainWindowMaximizeToggle() { }
        public void RequestMainWindowMinimize() { }
    }
}
