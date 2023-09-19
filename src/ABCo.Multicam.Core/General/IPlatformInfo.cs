using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.General
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
