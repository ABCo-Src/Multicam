namespace ABCo.Multicam.UI.Services
{
	public interface IUIWindow
    {
        bool CanMinimize { get; }
        bool CanMaximize { get; }
        bool CloseBtnRecommended { get; }
        bool BorderRecommended { get; }
        void CloseMainWindow();
        void RequestMainWindowMaximizeToggle();
        void RequestMainWindowMinimize();
    }
}
