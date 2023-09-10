using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers.Data;

namespace ABCo.Multicam.Core.Features.Switchers
{
	public class SwitcherConfigType : FeatureData
    {
        public override int DataId => SwitcherDataSpecs.CONFIG_TYPE;
		public SwitcherType Type { get; }
		public SwitcherConfigType(SwitcherType type) => Type = type;
	}

	public abstract class SwitcherConfig : FeatureData
    {
		public override int DataId => SwitcherDataSpecs.CONFIG_SPECIFIC;
    }

    public enum SwitcherType
    {
        Dummy,
        ATEM
    }
}
