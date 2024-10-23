namespace ABCo.Multicam.Server.Features.Switchers.Data.Config
{
    public class ATEMSwitcherConfig : SwitcherConfig
    {
        public override SwitcherType Type => SwitcherType.ATEM;

        public readonly string? IP;
        public ATEMSwitcherConfig(string? ip) => IP = ip;
    }
}
