using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
	public class SwitcherErrorException : Exception
    {
        public SwitcherErrorException(string message) : base(message) { }
    }

    public class UnexpectedSwitcherDisconnectionException : SwitcherErrorException
    {
        public UnexpectedSwitcherDisconnectionException() : base("Switcher was unexpectedly disconnected.") { }
    }

    public class SwitcherError : ServerData
    {
        public string? Message { get; }
        public SwitcherError(string? message) => Message = message;
    }
}
