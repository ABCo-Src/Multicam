using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers.Data;

namespace ABCo.Multicam.Core.Features.Switchers
{
	public abstract class SwitcherConfig : FeatureData
    {
		public override int DataId => SwitcherDataSpecs.CONFIG;
		public abstract SwitcherType Type { get; }
    }

    public enum SwitcherType
    {
        Dummy,
        ATEM
    }
}
