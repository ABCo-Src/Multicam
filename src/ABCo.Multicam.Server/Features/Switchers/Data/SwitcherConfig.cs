using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features.Switchers
{
	public abstract class SwitcherConfig : ServerData 
    {
        public abstract SwitcherType Type { get; }
    }

    public enum SwitcherType
    {
        Dummy,
        ATEM
    }
}
