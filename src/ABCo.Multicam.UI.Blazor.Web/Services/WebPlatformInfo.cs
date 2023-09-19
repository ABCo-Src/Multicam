using ABCo.Multicam.Core.General;

namespace ABCo.Multicam.UI.Blazor.Web.Services
{
	public class WebPlatformInfo : IPlatformInfo
	{
		public PlatformType GetPlatformType() => PlatformType.Web;
	}
}
