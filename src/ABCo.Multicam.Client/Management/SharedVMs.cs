using ABCo.Multicam.Client.Presenters;

namespace ABCo.Multicam.Client.Management
{
    public interface ISharedVMs
    {
        IPopOutVM PopOut { get; }
    }

    public class SharedVMs : ISharedVMs
    {
        IPopOutVM? _popOut;

        public IPopOutVM PopOut => _popOut ??= new PopOutVM();

        public SharedVMs(IClientInfo info) { }
    }
}
