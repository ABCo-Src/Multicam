using ABCo.Multicam.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Hosting.Scoping
{
    public interface IScopedPresenterStore<TPresenterParam> : IParameteredService<TPresenterParam>, IDisposable
    {
        TPresenter GetPresenter<TPresenter>(IScopeInfo info) where TPresenter : class, IUIPresenter, IParameteredService<TPresenterParam, IScopeInfo>;
        void OnDataChange(object obj);
    }

    public interface IUIPresenter
    {
        void Init();
        void OnDataChange(object obj);
    }

    public class ScopedPresenterStore<TPresenterParam> : IScopedPresenterStore<TPresenterParam>
    {
        IServiceSource _servSource;
        TPresenterParam _param;

        Dictionary<int, ScopePresenters> _registeredPresenters = new();

        public ScopedPresenterStore(TPresenterParam param, IServiceSource servSource)
        {
            _servSource = servSource;
            _param = param;

            // Make sure we're notified if a scope gets destroyed, so we can remove all the presenters registered with that scope in response.
            servSource.Get<IScopedConnectionManager>().ScopeDestroyed += OnScopeDestroy;
        }

        public void Dispose() => _servSource.Get<IScopedConnectionManager>().ScopeDestroyed -= OnScopeDestroy;

        public TPresenter GetPresenter<TPresenter>(IScopeInfo info) where TPresenter : class, IUIPresenter, IParameteredService<TPresenterParam, IScopeInfo>
        {
            // If there's nothing registered, add the item
            if (!_registeredPresenters.TryGetValue(info.ConnectionID, out ScopePresenters val))
            {
                var newVal = ConstructNew();
                _registeredPresenters.Add(info.ConnectionID, new ScopePresenters(info.Dispatcher, new List<IUIPresenter> { newVal }));
                newVal.Init();
                return newVal;
            }

            // If the item is in the registered list, return that
            for (int i = 0; i < val.Presenters.Count; i++)
                if (val.Presenters[i] is TPresenter presenter)
                    return presenter;

            // Otherwise, add it to the list
            {
                var newVal = ConstructNew();
                val.Presenters.Add(newVal);
                newVal.Init();
                return newVal;
            }

            TPresenter ConstructNew() => _servSource.Get<TPresenter, TPresenterParam, IScopeInfo>(_param, info);
        }

//        public void AddPresenter(IUIPresenter presenter, IScopeInfo info)
//        {
//            if (_registeredPresenters.TryGetValue(info.ConnectionID, out List<IUIPresenter>? val))
//            {
//#if DEBUG
//                // Assert there isn't already one of the same type
//                for (int i = 0; i < val.Count; i++)
//                    if (val[i].GetType() == presenter.GetType())
//                        throw new Exception("Type already registered!");
//#endif

//                _registeredPresenters.Add(info.ConnectionID, new List<IUIPresenter> { presenter });
//            }

//            else
//                _registeredPresenters.
//        }

        public void OnDataChange(object obj)
        {
            foreach (var list in _registeredPresenters.Values)
            {
                var presenters = list.Presenters;
				list.Dispatcher.QueueOnUIThread(() =>
                {
					for (int i = 0; i < presenters.Count; i++)
						presenters[i].OnDataChange(obj);
				});
			}
        }

        public record struct ScopePresenters(IMainThreadDispatcher Dispatcher, List<IUIPresenter> Presenters);

        void OnScopeDestroy(IScopeInfo obj) => _registeredPresenters.Remove(obj.ConnectionID);
    }
}
