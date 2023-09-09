namespace ABCo.Multicam.Core
{
	public interface INeedsNoInitialization { }

	public interface IParameteredServiceCollection
	{
		void AddSingleton<T, TTarget>() where T : class where TTarget : class, T;
        void AddTransient<T, TTarget>() where T : class where TTarget : class, T;
		void AddTransient<T, T1>(Func<T1, IServiceSource, T> factory);
        void AddTransient<T, T1, T2>(Func<T1, T2, IServiceSource, T> factory);
        void AddTransient<T, T1, T2, T3>(Func<T1, T2, T3, IServiceSource, T> factory);
	}

	public interface INeedsInitialization<T>
    {
        void FinishConstruction(T param1);
    }

    public interface INeedsInitialization<T1, T2>
    {
        void FinishConstruction(T1 param1, T2 param2);
    }

    public interface INeedsInitialization<T1, T2, T3>
    {
        void FinishConstruction(T1 param1, T2 param2, T3 param3);
    }

    public interface INeedsInitialization<T1, T2, T3, T4>
    {
        void FinishConstruction(T1 param1, T2 param2, T3 param3, T4 param4);
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
