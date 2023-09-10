using ABCo.Multicam.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class ServiceSource : IServiceSource
	{
		readonly IServiceProvider _container;
		public ServiceSource(IServiceProvider container) => _container = container;
		public T Get<T>() where T : class => _container.GetService<T>()!;

		public T Get<T, T1>(T1 param1) where T : class, IParameteredService<T1>
		{
			var factory = ParameteredTransientStore<T>.Factory ?? throw new Exception();
			var castedFactory = (Func<T1, IServiceSource, T>)factory;
			return castedFactory(param1, this);
		}

		public T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, IParameteredService<T1, T2>
		{
			var factory = ParameteredTransientStore<T>.Factory ?? throw new Exception();
			var castedFactory = (Func<T1, T2, IServiceSource, T>)factory;
			return castedFactory(param1, param2, this);
		}

		public T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, IParameteredService<T1, T2, T3>
		{
			var factory = ParameteredTransientStore<T>.Factory ?? throw new Exception();
			var castedFactory = (Func<T1, T2, T3, IServiceSource, T>)factory;
			return castedFactory(param1, param2, param3, this);
		}
	}

	public class TransientServiceRegister : IParameteredServiceCollection
	{
		readonly IServiceCollection _normalContainer;
		public TransientServiceRegister(IServiceCollection normalContainer) => _normalContainer = normalContainer;

		public void AddSingletonDirect<TTarget>() where TTarget : class
			=> _normalContainer.AddSingleton<TTarget>();

		public void AddSingleton<T, TTarget>()
			where T : class
			where TTarget : class, T
			=> _normalContainer.AddSingleton<T, TTarget>();

		public void AddTransient<T, TTarget>() 
			where T : class 
			where TTarget : class, T 
			=> _normalContainer.AddTransient<T, TTarget>();

		public void AddTransient<T, T1>(Func<T1, IServiceSource, T> factory) where T : IParameteredService<T1> => 
			ParameteredTransientStore<T>.Factory = factory;

		public void AddTransient<T, T1, T2>(Func<T1, T2, IServiceSource, T> factory) where T : IParameteredService<T1, T2> => 
			ParameteredTransientStore<T>.Factory = factory;

		public void AddTransient<T, T1, T2, T3>(Func<T1, T2, T3, IServiceSource, T> factory) where T : IParameteredService<T1, T2, T3> => 
			ParameteredTransientStore<T>.Factory = factory;
	}

	public static class ParameteredTransientStore<T>
	{
		public static Delegate? Factory = null;
	}
}
