using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core
{
    public interface IServiceSource
    {
        T Create<T>() where T : class;
        T CreateWithParent<T, TParent>(TParent parent) where T : class;
    }

    // NOTE: not unit tested, be careful when adding functionality.
    public class ServiceSource : IServiceSource
    {
        ServiceContainer _container;
        public ServiceSource(ServiceContainer container) => _container = container;
        public T Create<T>() where T : class => _container.GetInstance<T>();
        public T CreateWithParent<T, TParent>(TParent parent) where T : class => _container.GetInstance<TParent, T>(parent);
    }
}
