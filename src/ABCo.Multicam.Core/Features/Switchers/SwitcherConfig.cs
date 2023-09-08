namespace ABCo.Multicam.Core.Features.Switchers
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
