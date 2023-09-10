using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Data
{
    public class SwitcherState : FeatureData
	{
		public override int DataId => SwitcherFragmentID.STATE;

		public MixBlockState[] Data { get; }
		public SwitcherState(MixBlockState[] data) => Data = data;
	}
}
