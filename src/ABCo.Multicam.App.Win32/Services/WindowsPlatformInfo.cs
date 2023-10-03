using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.App.Win32.Services
{
	public class WindowsPlatformInfo : IPlatformInfo
	{
		public PlatformType GetPlatformType() => PlatformType.Windows;
	}
}
