using ABCo.Multicam.Core;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class ServiceSource : IServiceSource
	{
		readonly IServiceProvider _container;
		public ServiceSource(IServiceProvider container) => _container = container;
		public T Get<T>() where T : class => _container.GetService<T>()!;

		public T Get<T, T1>(T1 param1) where T : class, INeedsInitialization<T1>
		{
			var val = _container.GetService<T>();
			val!.FinishConstruction(param1);
			return val;
		}

		public T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, INeedsInitialization<T1, T2>
		{
			var val = _container.GetService<T>();
			val!.FinishConstruction(param1, param2);
			return val;
		}

		public T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, INeedsInitialization<T1, T2, T3>
		{
			var val = _container.GetService<T>();
			val!.FinishConstruction(param1, param2, param3);
			return val;
		}

		public Task<T> GetBackground<T, T1>(T1 param1) where T : class, INeedsInitialization<T1> =>
			Task.Run(() => Get<T, T1>(param1));
	}
}
