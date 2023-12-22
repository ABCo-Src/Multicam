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
        readonly IThreadDispatcher _mainThreadDispatcher;
        readonly IServerInfo _servSource;
        readonly IATEMPlatformCompatibility _compatibility;

        IATEMConnection? _connection;

        public ATEMSwitcher(ATEMSwitcherConfig config, IServerInfo servSource)
        {
            _config = config;
            _servSource = servSource;
            _compatibility = servSource.Get<IATEMPlatformCompatibility>();
            _mainThreadDispatcher = servSource.GetLocalClientConnection().Dispatcher;
        }

        public override SwitcherPlatformCompatibilityValue GetPlatformCompatibility() => _compatibility.GetCompatibility();

        public override void Connect()
        {
            // If the platform isn't supported, don't do anything and report that
            if (_compatibility.GetCompatibility() != SwitcherPlatformCompatibilityValue.Supported)
            {
                _eventHandler?.OnFailure(new SwitcherError("ATEM Switchers cannot currently be connected to, check the edit page for more info."));
				_mainThreadDispatcher.Queue(() => _eventHandler?.OnConnectionStateChange(false));
				return;
            }

            _connection = _servSource.Get<IATEMConnection, ATEMSwitcherConfig, IATEMSwitcher>(_config, this);
			_mainThreadDispatcher.Queue(() => _eventHandler?.OnConnectionStateChange(true));
		}

        public override void Disconnect()
        {
            if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

            _connection.Dispose();
            _connection = null;
            _mainThreadDispatcher.Queue(() => _eventHandler?.OnConnectionStateChange(false));
        }

        public override void RefreshConnectionStatus() => _eventHandler?.OnConnectionStateChange(_connection != null);

        public override void RefreshSpecs()
        {
            if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

            var newSpecs = _connection.InvalidateCurrentSpecs();
            _mainThreadDispatcher.Queue(() => _eventHandler?.OnSpecsChange(newSpecs));
        }

        public override void RefreshProgram(int mixBlock)
        {
            if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

            long val = _connection.GetProgram(mixBlock);
            _mainThreadDispatcher.Queue(() => _eventHandler?.OnProgramValueChange(new SwitcherProgramChangeInfo(mixBlock, (int)val, null)));
        }

        public override void RefreshPreview(int mixBlock)
        {
            if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();

            long val = _connection.GetPreview(mixBlock);
            _mainThreadDispatcher.Queue(() => _eventHandler?.OnPreviewValueChange(new SwitcherPreviewChangeInfo(mixBlock, (int)val, null)));
        }

        public override void SendProgramValue(int mixBlock, int id)
        {
            if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();
            _connection.SendProgram(mixBlock, id);
        }

        public override void SendPreviewValue(int mixBlock, int id)
        {
            if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();
            _connection.SendPreview(mixBlock, id);
        }

        public override void Cut(int mixBlock)
        {
            if (_connection == null) throw new UnexpectedSwitcherDisconnectionException();
            _connection.Cut(mixBlock);
        }

        public void ProcessError(Exception ex) => _mainThreadDispatcher.Queue(() => _eventHandler?.OnFailure(new(ex.Message)));

        public override void Dispose()
        {
            if (_connection != null) Disconnect();
        }
    }
}