using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Core.Features.Switchers
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
