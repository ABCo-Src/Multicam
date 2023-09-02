using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.General
{
	public interface IMainThreadDispatcher 
	{
		void QueueOnMainFeatureThread(Action act);
	}
}
