using ABCo.Multicam.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Blazor.Win32.Services
{
	public class WindowsPlatformInfo : IPlatformInfo
	{
		public PlatformType GetPlatformType() => PlatformType.Windows;
	}
}
