using ABCo.Multicam.Server.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Data
{
    // Represents information about how well-supported the current switcher mode is on this platform
    public class SwitcherCompatibility : ServerData
	{
		public SwitcherPlatformCompatibilityValue Value { get; }
		public SwitcherCompatibility(SwitcherPlatformCompatibilityValue value) => Value = value;
	}

	public enum SwitcherPlatformCompatibilityValue
	{
		Supported,
		UnsupportedPlatform,
		NoSoftware
	}
}
