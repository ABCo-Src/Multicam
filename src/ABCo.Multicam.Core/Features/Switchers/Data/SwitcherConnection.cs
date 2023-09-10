using ABCo.Multicam.Core.Features.Data;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
	public class SwitcherConnection : FeatureData
	{
		public override int DataId => SwitcherDataSpecs.CONNECTION;

		public bool IsConnected { get; }
		public SwitcherConnection(bool isConnected) => IsConnected = isConnected;
	}
}
