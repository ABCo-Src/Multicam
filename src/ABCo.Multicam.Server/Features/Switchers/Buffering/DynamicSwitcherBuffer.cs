namespace ABCo.Multicam.Server.Features.Switchers.Buffering
{
	public interface IDynamicSwitcherBuffer : IServerService<SwitcherConfig>, IDisposable
    {
        void SetEventHandler(ISwitcherEventHandler? handler);
        void ChangeSwitcher(SwitcherConfig config);
        ISwitcherBuffer CurrentBuffer { get; }
    }

    public class DynamicSwitcherBuffer : IDynamicSwitcherBuffer
    {
        readonly IServerInfo _info;
        ISwitcherEventHandler? _handler;

        public ISwitcherBuffer CurrentBuffer { get; private set; }

        public DynamicSwitcherBuffer(SwitcherConfig config, IServerInfo info)
        {
            _info = info;
            CurrentBuffer = new SwitcherBuffer(config, info);
        }

        public void ChangeSwitcher(SwitcherConfig config)
        {
            CurrentBuffer.Dispose();
            CurrentBuffer = new SwitcherBuffer(config, _info);
            CurrentBuffer.SetEventHandler(_handler);

            // Refresh everything to match the change
            _handler?.OnConnectionStateChange(CurrentBuffer.IsConnected);
            _handler?.OnSpecsChange(CurrentBuffer.Specs);
        }

        public void SetEventHandler(ISwitcherEventHandler? handler)
        {
            _handler = handler;
            CurrentBuffer.SetEventHandler(handler);
        }

        public void Dispose() => CurrentBuffer.Dispose();
    }
}
