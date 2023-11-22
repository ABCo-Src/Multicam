using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features.Switchers
{
	public abstract class SwitcherConfig
    {
        public abstract SwitcherType Type { get; }
    }

    public enum SwitcherType
    {
        Dummy,
        ATEM
    }
}
