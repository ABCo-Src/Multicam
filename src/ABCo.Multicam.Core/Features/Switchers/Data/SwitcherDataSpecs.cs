using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers.Data.Config;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.Features.Switchers.Data;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
	public record class BusChangeInfo(int MB, int Val);

    public static class SwitcherDataSpecs
    {
        public const int GENERALINFO = 0;
        public const int CONFIG_TYPE = 1;
        public const int CONFIG_SPECIFIC = 2;
        public const int CONNECTION = 3;
        public const int SPECS = 4;
        public const int STATE = 5;
        public const int PREVIOUS_ERROR = 6;

        public static FeatureDataInfo[] DataInfo => new FeatureDataInfo[]
        {
            new FeatureDataInfo(typeof(FeatureGeneralInfo), new FeatureGeneralInfo(FeatureTypes.Switcher, "New Switcher")),
            new FeatureDataInfo(typeof(SwitcherConfigType), new SwitcherConfigType(SwitcherType.Dummy)),
            new FeatureDataInfo(typeof(SwitcherConfig), new DummySwitcherConfig(4)),
            new FeatureDataInfo(typeof(SwitcherSpecs), new SwitcherSpecs()),
            new FeatureDataInfo(typeof(SwitcherState), new SwitcherState(Array.Empty<MixBlockState>())),
            new FeatureDataInfo(typeof(SwitcherConnection), new SwitcherConnection(false)),
            new FeatureDataInfo(typeof(SwitcherError), new SwitcherError(null)),
            new FeatureDataInfo(typeof(SwitcherCompatibility), new SwitcherCompatibility(SwitcherPlatformCompatibilityValue.Supported))
        };
	}

    public static class SwitcherActionID
    {
        public const int SET_GENERALINFO = 0;
        public const int ACKNOWLEDGE_ERROR = 1;
        public const int CONNECT = 2;
        public const int DISCONNECT = 3;
		public const int SET_CONFIG_TYPE = 4;
		public const int SET_CONFIG = 5;
		public const int SET_PROGRAM = 6;
        public const int SET_PREVIEW = 7;
        public const int CUT = 8;
    }
}
