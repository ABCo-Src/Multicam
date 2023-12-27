using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.General.Queues;

namespace ABCo.Multicam.Server.Features.Switchers.Core.ATEM
{
    public interface IATEMSwitcher : IRawSwitcher, IErrorHandlingTarget, IServerService<ATEMSwitcherConfig>
    {
    }

    public class ATEMSwitcher : RawSwitcher, IATEMSwitcher
    {
        readonly ATEMSwitcherConfig _config;
        readonly IThreadDispatcher _dispatcher;
        readonly IServerInfo _info;
        readonly IATEMPlatformCompatibility _compatibility;
        readonly BackgroundThreadExecutionBuffer _buffer;

        IATEMConnection? _connection; // Only access with the background thread!

        public ATEMSwitcher(ATEMSwitcherConfig config, IServerInfo info)
        {
            _config = config;
            _info = info;
			_compatibility = new ATEMPlatformCompatibility(info);
            _dispatcher = info.Dispatcher;
            _buffer = new(ProcessError);
        }

        public override SwitcherPlatformCompatibilityValue GetPlatformCompatibility() => _compatibility.GetCompatibility();

        public override void Connect()
        {
            // If the platform isn't supported, don't do anything and report that
            if (_compatibility.GetCompatibility() != SwitcherPlatformCompatibilityValue.Supported)
            {
                _eventHandler?.OnFailure(new SwitcherError("ATEM Switchers cannot currently be connected to, check the edit page for more info."));
				_dispatcher.Queue(() => _eventHandler?.OnConnectionStateChange(false));
				return;
            }

            _buffer.QueueTask(() =>
            {
                _connection = new ATEMConnection(_config, this, _info);
                _dispatcher.Queue(() => _eventHandler?.OnConnectionStateChange(true));
            });
		}

        public override void Disconnect()
        {
            _buffer.QueueTask(() =>
            {
                if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

                _connection.Dispose();
                _connection = null;
                _dispatcher.Queue(() => _eventHandler?.OnConnectionStateChange(false));
            });
        }

        public override void RefreshConnectionStatus() => _eventHandler?.OnConnectionStateChange(_connection != null);

        public override void RefreshSpecs()
        {
            _buffer.QueueTask(() =>
            {
                if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

                var newSpecs = _connection.InvalidateCurrentSpecs();
                _dispatcher.Queue(() => _eventHandler?.OnSpecsChange(newSpecs));
            });
        }

        public override void RefreshProgram(int mixBlock)
        {
            _buffer.QueueTask(() =>
            {
                if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

                long val = _connection.GetProgram(mixBlock);
                _dispatcher.Queue(() => _eventHandler?.OnProgramValueChange(new SwitcherProgramChangeInfo(mixBlock, (int)val, null)));
            });
        }

        public override void RefreshPreview(int mixBlock)
        {
            _buffer.QueueTask(() =>
            {
                if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

                long val = _connection.GetPreview(mixBlock);
                _dispatcher.Queue(() => _eventHandler?.OnPreviewValueChange(new SwitcherPreviewChangeInfo(mixBlock, (int)val, null)));
            });
        }

        public override void SendProgramValue(int mixBlock, int id)
        {
            _buffer.QueueTask(() =>
            {
                if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();
                _connection.SendProgram(mixBlock, id);
            });
        }

        public override void SendPreviewValue(int mixBlock, int id)
        {
            _buffer.QueueTask(() =>
            {
                if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();
                _connection.SendPreview(mixBlock, id);
            });
        }

        public override void Cut(int mixBlock)
        {
            _buffer.QueueTask(() =>
            {
                if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();
                _connection.Cut(mixBlock);
            });
        }

        public void ProcessError(Exception ex) => _dispatcher.Queue(() => _eventHandler?.OnFailure(new(ex.Message)));

        public override void Dispose()
        {
            if (_connection != null) Disconnect();
        }
    }
}