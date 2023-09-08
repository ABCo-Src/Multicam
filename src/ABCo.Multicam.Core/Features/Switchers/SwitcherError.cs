namespace ABCo.Multicam.Core.Features.Switchers
{
	public class SwitcherErrorException : Exception
	{
		public SwitcherErrorException(string message) : base(message) { }
	}

	public class UnexpectedSwitcherDisconnectionException : SwitcherErrorException
	{
		public UnexpectedSwitcherDisconnectionException() : base("Switcher was unexpectedly disconnected.") { }
	}

	public record struct SwitcherError(Exception Exception);
}
