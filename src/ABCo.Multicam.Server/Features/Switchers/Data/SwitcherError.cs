namespace ABCo.Multicam.Server.Features.Switchers.Data
{
	public class SwitcherErrorException : Exception
    {
        public SwitcherErrorException(string message) : base(message) { }
    }

    public class UnexpectedSwitcherDisconnectionException : SwitcherErrorException
    {
        public UnexpectedSwitcherDisconnectionException() : base("Switcher was unexpectedly disconnected.") { }
    }

    public class SwitcherError
    {
        public string? Message { get; }
        public SwitcherError(string? message) => Message = message;
    }
}
