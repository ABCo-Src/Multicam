using ABCo.Multicam.Core.General;
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
		ATEMPlatformCompatibilityValue GetCompatibility();
	}

	public class ATEMPlatformCompatibility : IATEMPlatformCompatibility
	{
		readonly IPlatformInfo _info;
		public ATEMPlatformCompatibility(IServiceSource servSource) => _info = servSource.Get<IPlatformInfo>();

		[SupportedOSPlatform("windows")]
		public ATEMPlatformCompatibilityValue GetCompatibility()
		{
			if (_info.GetPlatformType() != PlatformType.Windows) return ATEMPlatformCompatibilityValue.UnsupportedPlatform;

			try
			{
				var discovery = new CBMDSwitcherDiscovery();
				Marshal.ReleaseComObject(discovery);
				return ATEMPlatformCompatibilityValue.Supported;
			}
			catch (COMException)
			{
				return ATEMPlatformCompatibilityValue.NoSoftware;
			}
		}
	}

	public enum ATEMPlatformCompatibilityValue
	{
		Supported,
		UnsupportedPlatform,
		NoSoftware
	}
}
