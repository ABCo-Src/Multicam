using ABCo.Multicam.Client.Services;

namespace ABCo.Multicam.Client.Blazor.Services
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
