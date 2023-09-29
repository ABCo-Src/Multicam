using ABCo.Multicam.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ABCo.Multicam.UI.Blazor.Services
{
	public class AppWideServiceRegister
	{
		static List<KeyValuePair<Type, Type>> _registeredScoped = new();

		public static IParameteredServiceCollection GetServiceBuilder() => new RegisterBuilder();

		public static void AddRegisteredScopedServicesTo(IServiceCollection collection, bool asSingletons = false)
		{
			if (asSingletons)
				for (int i = 0; i < _registeredScoped.Count; i++)
					collection.AddSingleton(_registeredScoped[i].Key, _registeredScoped[i].Value);
			else
				for (int i = 0; i < _registeredScoped.Count; i++)
					collection.AddScoped(_registeredScoped[i].Key, _registeredScoped[i].Value);
		}

		public static IServiceSource GetWithScopesFrom(IServiceProvider provider) => new ServiceSource(provider);

		// Source
		class ServiceSource : IServiceSource
		{
			readonly IServiceProvider _containerForScoped;
			public ServiceSource(IServiceProvider containerForScoped) => _containerForScoped = containerForScoped;
			public T Get<T>() where T : class
			{
				// Check if it's a singleton
				if (AppWideServiceRegisterSingletonStore<T>.Factory != null)
				{
					if (AppWideServiceRegisterSingletonStore<T>.Object != null)
						return (T)AppWideServiceRegisterSingletonStore<T>.Object;
					else
					{
						var res = AppWideServiceRegisterSingletonStore<T>.Factory(this);
						AppWideServiceRegisterSingletonStore<T>.Object = res;
						return res;
					}
				}

				// Check if it's a transient
				if (AppWideServiceRegisterTransientStore<T>.Factory != null) return ((Func<IServiceSource, T>)AppWideServiceRegisterTransientStore<T>.Factory)(this);

				// Check if it's a scoped service
				var scopeRes = _containerForScoped.GetService<T>();
				if (scopeRes != null) return scopeRes;

				// If it's none, then it's not registered
				throw new Exception("Unregistered service requested!");
			}

			public T Get<T, T1>(T1 param1) where T : class, IParameteredService<T1>
			{
				var factory = AppWideServiceRegisterTransientStore<T>.Factory ?? throw new Exception();
				var castedFactory = (Func<T1, IServiceSource, T>)factory;
				return castedFactory(param1, this);
			}

			public T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, IParameteredService<T1, T2>
			{
				var factory = AppWideServiceRegisterTransientStore<T>.Factory ?? throw new Exception();
				var castedFactory = (Func<T1, T2, IServiceSource, T>)factory;
				return castedFactory(param1, param2, this);
			}

			public T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, IParameteredService<T1, T2, T3>
			{
				var factory = AppWideServiceRegisterTransientStore<T>.Factory ?? throw new Exception();
				var castedFactory = (Func<T1, T2, T3, IServiceSource, T>)factory;
				return castedFactory(param1, param2, param3, this);
			}
		}

		// Builder
		class RegisterBuilder : IParameteredServiceCollection
		{
			public void AddSingleton<T>(Func<IServiceSource, T> val)
				where T : class
				=> AppWideServiceRegisterSingletonStore<T>.Factory = val;

			public void AddScoped<T, TTarget>() where T : class where TTarget : class, T
				=> _registeredScoped.Add(new(typeof(T), typeof(TTarget)));

			public void AddTransient<T>(Func<IServiceSource, T> f) where T : class
				=> AppWideServiceRegisterTransientStore<T>.Factory = f;

			public void AddTransient<T, T1>(Func<T1, IServiceSource, T> factory) where T : IParameteredService<T1> =>
				AppWideServiceRegisterTransientStore<T>.Factory = factory;

			public void AddTransient<T, T1, T2>(Func<T1, T2, IServiceSource, T> factory) where T : IParameteredService<T1, T2> =>
				AppWideServiceRegisterTransientStore<T>.Factory = factory;

			public void AddTransient<T, T1, T2, T3>(Func<T1, T2, T3, IServiceSource, T> factory) where T : IParameteredService<T1, T2, T3> =>
				AppWideServiceRegisterTransientStore<T>.Factory = factory;
		}
	}

	public static class AppWideServiceRegisterSingletonStore<T>
	{
		public static object? Object = null;
		public static Func<IServiceSource, T>? Factory = null;
	}

	public static class AppWideServiceRegisterTransientStore<T>
	{
		public static Delegate? Factory = null;
	}
}
