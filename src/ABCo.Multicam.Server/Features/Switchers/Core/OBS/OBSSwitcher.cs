using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Features.Switchers.Fading;
using ABCo.Multicam.Server.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS
{
    public partial class OBSSwitcher : RawSwitcher
	{
		// Implementation of: https://github.com/obsproject/obs-websocket/blob/master/docs/generated/protocol.md
		readonly OBSSwitcherConfig _config;
		OBSConnection? _connection;

		public OBSSwitcher(OBSSwitcherConfig config)
		{
			_config = config;
		}

		public override SwitcherPlatformCompatibilityValue GetPlatformCompatibility() => SwitcherPlatformCompatibilityValue.Supported;

		public override async void Connect()
		{
			// TODO: Slamming protection?
			try
			{
				// Connect to OBS
				_connection?.Dispose();
				_connection = await OBSConnection.ConnectAndGetInfo(_config);
				_eventHandler?.OnConnectionStateChange(true);
				OBSEventLoop();
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override void Disconnect()
		{
			try
			{
				if (_connection == null) throw ThrowDisconnected();

				_connection?.Dispose();
				_connection = null;
				_eventHandler?.OnConnectionStateChange(false);
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		async void OBSEventLoop()
		{
			if (_connection == null) throw ThrowDisconnected();

			var code = OBSSwitcherAction.None;
			while (code != OBSSwitcherAction.Disconnected)
			{
				//try
				//{
					code = await _connection.ReadData();

					// Perform the appropriate action
					switch (code)
					{
						case OBSSwitcherAction.PreviewChanged:
							_eventHandler?.OnPreviewValueChange(new SwitcherPreviewChangeInfo(0, _connection.LookupCurrentPreviewId(), new RetrospectiveFadeInfo()));
							break;
						case OBSSwitcherAction.ProgramChanged:
							_eventHandler?.OnProgramValueChange(new SwitcherProgramChangeInfo(0, _connection.LookupCurrentProgramId(), new RetrospectiveFadeInfo()));
							break;
						case OBSSwitcherAction.NotifySpecsChanged:
							_eventHandler?.OnSpecsChange(_connection.CreateSpecs());
							break;
					}
				//}
				//catch (Exception ex) { HandleFail(ex); }
			}

			// Disconnect now that we're finished.
			_connection = null;
		}

		public override void RefreshSpecs()
		{
			try
			{
				if (_connection == null) throw ThrowDisconnected();
				_eventHandler?.OnSpecsChange(_connection.CreateSpecs());
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override void RefreshConnectionStatus() => _eventHandler?.OnConnectionStateChange(_connection != null);

		public override void RefreshProgram(int mixBlock)
		{
			try
			{
				if (_connection == null) throw ThrowDisconnected();
				_eventHandler?.OnProgramValueChange(new(0, _connection.LookupCurrentProgramId(), new()));
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override void RefreshPreview(int mixBlock)
		{
			try
			{
				if (_connection == null) throw ThrowDisconnected();
				_eventHandler?.OnPreviewValueChange(new(0, _connection.LookupCurrentPreviewId(), new()));
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override async void SendProgramValue(int mixBlock, int id)
		{
			try
			{
				if (_connection == null) throw ThrowDisconnected();
				await _connection.SetProgram(mixBlock, id);
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override async void SendPreviewValue(int mixBlock, int id)
		{
			try
			{
				if (_connection == null) throw ThrowDisconnected();
				await _connection.SetPreview(mixBlock, id);
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override void Dispose() => _connection?.Dispose();
		void HandleFail(Exception ex) => _eventHandler?.OnFailure(new SwitcherError(ex.Message));

		static Exception ThrowDisconnected() => new OBSCommunicationException("Unexpected disconnection from OBS.");
	}
}
