using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core
{
	/// <summary>
	/// A switcher that performs all operations in the background
	/// </summary>
	public class BackgroundThreadSwitcher : RawSwitcher
	{
		IRawSwitcher _innerSwitcher;

		public BackgroundThreadSwitcher(IRawSwitcher switcher)
		{

		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}

		public override SwitcherPlatformCompatibilityValue GetPlatformCompatibility()
		{
			throw new NotImplementedException();
		}

		public override void RefreshConnectionStatus()
		{
			throw new NotImplementedException();
		}

		public override void RefreshSpecs()
		{
			throw new NotImplementedException();
		}
	}
}
