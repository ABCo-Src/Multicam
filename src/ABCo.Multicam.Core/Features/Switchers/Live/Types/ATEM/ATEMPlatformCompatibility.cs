using ABCo.Multicam.Core.General;
using ABCo.Multicam.Server.Features.Switchers.Data;
using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Live.Types.ATEM
{
	public interface IATEMPlatformCompatibility
	{
		SwitcherPlatformCompatibilityValue GetCompatibility();
	}

	public class ATEMPlatformCompatibility : IATEMPlatformCompatibility
	{
		readonly IPlatformInfo _info;
		public ATEMPlatformCompatibility(IServerInfo servSource) => _info = servSource.Get<IPlatformInfo>();

		[SupportedOSPlatform("windows")]
		public SwitcherPlatformCompatibilityValue GetCompatibility()
		{
			if (_info.GetPlatformType() != PlatformType.Windows) return SwitcherPlatformCompatibilityValue.UnsupportedPlatform;

			try
			{
				var discovery = new CBMDSwitcherDiscovery();
				Marshal.ReleaseComObject(discovery);
				return SwitcherPlatformCompatibilityValue.Supported;
			}
			catch (COMException)
			{
				return SwitcherPlatformCompatibilityValue.NoSoftware;
			}
		}
	}
}
