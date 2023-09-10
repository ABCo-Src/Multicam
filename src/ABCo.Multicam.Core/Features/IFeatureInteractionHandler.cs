using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features
{
	public interface IFeatureInteractionHandler : IDisposable
	{
		void PerformAction(int id);
		void PerformAction(int id, object param);
		void SetFragmentChangeHandler(IFragmentChangeEventHandler? eventHandler);
		void RefreshData<T>() where T : FeatureData;
	}

	public interface ILocalFragmentCollection
	{
		T GetData<T>() where T : FeatureData;
		void SetData(FeatureData val);
	}

	public interface IFragmentChangeEventHandler
	{
		void OnDataChange(FeatureData val);
	}
}
