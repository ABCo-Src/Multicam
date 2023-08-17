using LightInject;

namespace ABCo.Multicam.Core
{
    public interface INeedsNoInitialization { }

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

    public interface IServiceSource
    {
        T Get<T>() where T : class;
        T Get<T, T1>(T1 param1) where T : class, INeedsInitialization<T1>;
        T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, INeedsInitialization<T1, T2>;
        T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, INeedsInitialization<T1, T2, T3>;
    }

    public class ServiceSource : IServiceSource
    {
        readonly ServiceContainer _container;
        public ServiceSource(ServiceContainer container) => _container = container;
        public T Get<T>() where T : class => _container.GetInstance<T>();

        public T Get<T, T1>(T1 param1) where T : class, INeedsInitialization<T1>
        {
            var val = _container.GetInstance<T>();
            val.FinishConstruction(param1);
            return val;
        }

        public T Get<T, T1, T2>(T1 param1, T2 param2) where T : class, INeedsInitialization<T1, T2>
        {
            var val = _container.GetInstance<T>();
            val.FinishConstruction(param1, param2);
            return val;
        }

        public T Get<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : class, INeedsInitialization<T1, T2, T3>
        {
            var val = _container.GetInstance<T>();
            val.FinishConstruction(param1, param2, param3);
            return val;
        }
    }
}
