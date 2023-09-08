using ABCo.Multicam.Core.Features.Switchers;

namespace ABCo.Multicam.Core.General
{
	public class FeatureDataStore
	{
		readonly SwitcherFeatureDataSpecification _data;
		readonly Data[] _fragments;

		public FeatureDataStore(SwitcherFeatureDataSpecification data)
		{
			_data = data;

			_fragments = new Data[data.Fragments.Length];
			for (int i = 0; i < data.Fragments.Length; i++)
			{
				_fragments[i] = new Data() 
				{ 
					Id = data.Fragments[i].Id,
					Object = null
				};
			}
		}

		public object? GetStoredValue(int id)
		{
			for (int i = 0; i < _fragments.Length; i++)			
				if (_fragments[i].Id == id)
					return _fragments[i].Object;

			throw new Exception("Not registered!");
		}

		public Action<object> GetApplyToLive(int id)
		{
			for (int i = 0; i < _data.ParameterlessActions.Length; i++)
				if (_data.ParameterlessActions[i].Id == id)
					return _data.ParameterlessActions[i].ApplyToLive;

			throw new Exception("Not registered!");
		}

		public Action<object, object> GetApplyToLiveParamed(int id)
		{
			for (int i = 0; i < _data.ParamedActions.Length; i++)
				if (_data.ParamedActions[i].Id == id)
					return _data.ParamedActions[i].ApplyToLive;

			throw new Exception("Not registered!");
		}

		struct Data
		{
			public int Id;
			public object? Object;
		}
	}
}
