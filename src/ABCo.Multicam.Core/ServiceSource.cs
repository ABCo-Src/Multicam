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
        T Get<T>() where T : class;
    }

    public class ServiceSource : IServiceSource
    {
        ServiceContainer _container;
        public ServiceSource(ServiceContainer container) => _container = container;
        public T Get<T>() where T : class => _container.GetInstance<T>();
    }
}
