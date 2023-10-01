using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.Server.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Data
{
	public class FeaturesList : ServerData
	{
		public IList<IServerTarget> Features { get; }
		public FeaturesList(IList<IServerTarget> features) => Features = features;
	}
}
