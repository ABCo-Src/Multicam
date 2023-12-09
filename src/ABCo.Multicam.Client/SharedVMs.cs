using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
