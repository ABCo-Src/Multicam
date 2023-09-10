using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
    public class SwitcherConnection : FeatureData
	{
		public override int DataId => SwitcherFragmentID.CONNECTION;

		public bool IsConnected { get; }
		public SwitcherConnection(bool isConnected) => IsConnected = isConnected;
	}
}
