using ABCo.Multicam.Core.Features.Switchers.Data;

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

	public class SwitcherError : FeatureData 
	{
		public override int DataId => SwitcherFragmentID.PREVIOUS_ERROR;

		public string? Message { get; }
		public SwitcherError(string? message) => Message = message;
	}
}
