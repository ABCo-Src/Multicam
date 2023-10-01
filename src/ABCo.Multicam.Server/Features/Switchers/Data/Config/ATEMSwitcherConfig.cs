using ABCo.Multicam.Server.Features.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Data.Config
{
	public class ATEMSwitcherConfig : SwitcherConfig
	{
		public readonly string? IP;
		public ATEMSwitcherConfig(string? ip) => IP = ip;
	}
}
