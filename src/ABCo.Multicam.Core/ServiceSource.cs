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
    }

    public class ServiceSource : IServiceSource
    {
        readonly ServiceContainer _container;
        public ServiceSource(ServiceContainer container) => _container = container;
        public T Get<T>() where T : class => _container.GetInstance<T>();
    }
}
