using ABCo.Multicam.Client.Presenters;

namespace ABCo.Multicam.Client
{
	public interface ISharedVMs
	{
		IPopOutVM PopOut { get; }
	}

	public class SharedVMs : ISharedVMs
	{
		public IPopOutVM PopOut { get; }

		public SharedVMs(IClientInfo info) => PopOut = new PopOutVM();
	}
}
