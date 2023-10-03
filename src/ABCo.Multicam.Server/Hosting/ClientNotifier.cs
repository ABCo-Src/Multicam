using ABCo.Multicam.Server;
using ABCo.Multicam.Server.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting
{
    public interface IRemoteClientNotifier
    {
        T GetOrAddClientEndpoint<T>(IClientInfo info) where T : class, IClientNotificationTarget;
    }

    public interface IClientNotifier : IRemoteClientNotifier, IServerService<IServerTarget>, IDisposable
    {
        void OnDataChange(ServerData obj);
    }

    public interface IConnectedClientsBoundClientNotifier : IClientNotifier
    {
        void OnClientDisconnect(IClientInfo obj);
        void SetDisposeAction(Action act);
    }

    public interface IClientNotificationTarget : IClientService<IServerTarget>
    {
        void Init();
        void OnDataChange(ServerData obj);
    }

    public abstract class ServerData { }

    public class ClientNotifier : IConnectedClientsBoundClientNotifier
    {
        Action _onDispose = () => { };
        readonly IServerTarget _dispatchedTarget;

        Dictionary<int, ScopePresenters> _registeredPresenters = new();

        public ClientNotifier(IServerTarget target, IServerInfo info)
        {
            // Wrap the target up in a dispatcher service, so everything the clients ask for gets dispatched back to the server thread properly
            _dispatchedTarget = info.Get<IDispatchingServerTarget, IServerTarget>(target);
        }

        public void SetDisposeAction(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => _onDispose();

        // === This method should work from any thread (server, client etc.) ===
        public T GetOrAddClientEndpoint<T>(IClientInfo info) where T : class, IClientNotificationTarget
        {
            lock (this)
            {
                // If there's nothing registered, add the item
                if (!_registeredPresenters.TryGetValue(info.ConnectionID, out ScopePresenters val))
                {
                    var newVal = ConstructNew();
                    _registeredPresenters.Add(info.ConnectionID, new ScopePresenters(info.Dispatcher, new List<IClientNotificationTarget> { newVal }));
                    newVal.Init();
                    return newVal;
                }

                // If the item is in the registered list, return that
                for (int i = 0; i < val.Presenters.Count; i++)
                    if (val.Presenters[i] is T presenter)
                        return presenter;

                // Otherwise, add it to the list
                {
                    var newVal = ConstructNew();
                    val.Presenters.Add(newVal);
                    newVal.Init();
                    return newVal;
                }

                T ConstructNew() => info.Get<T, IServerTarget>(_dispatchedTarget);
            }
        }

        public void OnDataChange(ServerData obj)
        {
            lock (this)
            {
				foreach (var list in _registeredPresenters.Values)
				{
					// Thread safety: It's better to have the for inside the dispatch, as the only way the list can be
					// changed is *by* the client appending to it (which the loop is fine with), so we don't get cross-thread accesses like this
					var presenters = list.Presenters;
					list.Dispatcher.Queue(() =>
					{
						for (int i = 0; i < presenters.Count; i++)
							presenters[i].OnDataChange(obj);
					});
				}
			}
        }

        public record struct ScopePresenters(IThreadDispatcher Dispatcher, List<IClientNotificationTarget> Presenters);

        public void OnClientDisconnect(IClientInfo obj) => _registeredPresenters.Remove(obj.ConnectionID);
    }
}
