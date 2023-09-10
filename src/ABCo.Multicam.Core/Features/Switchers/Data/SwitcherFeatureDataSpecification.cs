using ABCo.Multicam.Core.Features.Switchers.Types;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
    public record class BusChangeInfo(int MB, int Val);

    public static class SwitcherFragmentID
    {
        public const int GENERALINFO = 0;
        public const int CONFIG = 1;
        public const int CONNECTION = 2;
        public const int SPECS = 3;
        public const int STATE = 4;
        public const int PREVIOUS_ERROR = 5;
    }

    public static class SwitcherActionID
    {
        public const int SET_GENERALINFO = 0;
        public const int CONNECT = 1;
        public const int DISCONNECT = 2;
        public const int SET_PROGRAM = 3;
        public const int SET_PREVIEW = 4;
        public const int CUT = 5;
        public const int SET_SWITCHER = 6;
        public const int ACKNOWLEDGE_ERROR = 7;
    }
}
