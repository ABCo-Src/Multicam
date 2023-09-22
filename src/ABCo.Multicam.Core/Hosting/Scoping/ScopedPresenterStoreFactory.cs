using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Hosting.Scoping
{
    public interface IScopedPresenterStoreFactory
    {
        IScopedPresenterStore<T> Get<T>(T param);
    }

    public class ScopedPresenterStoreFactory : IScopedPresenterStoreFactory
    {
        IServiceSource _servSource;
        public ScopedPresenterStoreFactory(IServiceSource servSource) => _servSource = servSource;
        public IScopedPresenterStore<T> Get<T>(T param) => new ScopedPresenterStore<T>(param, _servSource);
    }
}
