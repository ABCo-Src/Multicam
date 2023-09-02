using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.General
{
	public interface IOrderedBackgroundQueue 
	{
		void QueueTask<T>(Action<T> act, T val);
	}

	public interface ISwitcherOrderedBackgroundQueue : IOrderedBackgroundQueue { }

	public class OrderedBackgroundQueue : IOrderedBackgroundQueue, ISwitcherOrderedBackgroundQueue
	{
		public void QueueTask<T>(Action<T> act, T val)
		{
			ThreadPool.QueueUserWorkItem(act, val, false);
		}
	}
}
