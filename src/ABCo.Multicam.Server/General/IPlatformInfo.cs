namespace ABCo.Multicam.Server.General
{
    public interface IPlatformInfo
    {
        PlatformType GetPlatformType();
    }

    public enum PlatformType
    {
        Windows,
        Web
    }
}
