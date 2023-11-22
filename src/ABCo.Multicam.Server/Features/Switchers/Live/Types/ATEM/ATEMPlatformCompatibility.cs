using ABCo.Multicam.Server.General;
using BMDSwitcherAPI;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ABCo.Multicam.Server.Features.Switchers.Live.Types.ATEM
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
