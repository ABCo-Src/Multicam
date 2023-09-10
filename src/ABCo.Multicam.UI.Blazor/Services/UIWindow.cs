using ABCo.Multicam.UI.Services;

namespace ABCo.Multicam.UI.Blazor.Services
{
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
