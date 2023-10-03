using ABCo.Multicam.Server.Hosting.Clients;

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
