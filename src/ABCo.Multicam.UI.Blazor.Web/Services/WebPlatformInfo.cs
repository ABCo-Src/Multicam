using ABCo.Multicam.Server.General;

namespace ABCo.Multicam.Client.Blazor.Web.Services
{
	public class WebPlatformInfo : IPlatformInfo
	{
		public PlatformType GetPlatformType() => PlatformType.Web;
	}
}
