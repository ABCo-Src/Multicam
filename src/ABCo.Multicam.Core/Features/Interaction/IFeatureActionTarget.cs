using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Interaction
{
	public interface IFeatureActionTarget : IDisposable
	{
		void PerformAction(int id);
		void PerformAction(int id, object param);
	}
}
