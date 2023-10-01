using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.Hosting;

namespace ABCo.Multicam.Server.Features.Switchers
{
    public class SwitcherConfigType : ServerData
    {
		public SwitcherType Type { get; }
		public SwitcherConfigType(SwitcherType type) => Type = type;
	}

	public abstract class SwitcherConfig : ServerData { }

    public enum SwitcherType
    {
        Dummy,
        ATEM
    }
}
