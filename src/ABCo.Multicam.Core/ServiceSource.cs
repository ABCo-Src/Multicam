using System.Diagnostics.CodeAnalysis;

namespace ABCo.Multicam.Core
{
	public interface IParameteredServiceCollection
	{
		void AddSingletonDirect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTarget>() where TTarget : class;
		void AddSingleton<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTarget>() where T : class where TTarget : class, T;
        void AddTransient<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTarget>() where T : class where TTarget : class, T;
		void AddTransient<T, T1>(Func<T1, IServiceSource, T> factory) where T : IParameteredService<T1>;
        void AddTransient<T, T1, T2>(Func<T1, T2, IServiceSource, T> factory) where T : IParameteredService<T1, T2>;
        void AddTransient<T, T1, T2, T3>(Func<T1, T2, T3, IServiceSource, T> factory) where T : IParameteredService<T1, T2, T3>;
	}

    public interface IParameteredService<T>
    {
    }

	public interface IParameteredService<T, T2>
	{
	}

	public interface IParameteredService<T, T2, T3>
	{
	}

	public interface IServiceSource
    {
        T Get<T>() where T : class;
        T Get<T, T1>(T1 param1) where T : class, IParameteredService<T1>;
        T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, IParameteredService<T1, T2>;
        T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, IParameteredService<T1, T2, T3>;
    }
}
