using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Features.Switchers.Fading;
using ABCo.Multicam.Server.General;
using ABCo.Multicam.Server.General.Queues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS
{
    public partial class OBSSwitcher : RawSwitcher
	{
		// Implementation of: https://github.com/obsproject/obs-websocket/blob/master/docs/generated/protocol.md
		readonly OBSSwitcherConfig _config;
		readonly IExecutionBuffer<OBSSwitcher> _buffer;
		OBSConnection? _connection;

		public OBSSwitcher(OBSSwitcherConfig config)
		{
			_config = config;
			_buffer = new SameThreadExecutionBuffer<OBSSwitcher>(this, HandleFail);
		}

		public override SwitcherPlatformCompatibilityValue GetPlatformCompatibility() => SwitcherPlatformCompatibilityValue.Supported;

		public override void Connect()
		{
			_buffer.QueueTaskAsync(async () =>
			{
				_connection?.Dispose();
				_connection = await OBSConnection.ConnectAndGetInfo(_config);
				_eventHandler?.OnConnectionStateChange(true);
				ReadMessageThenQueue();
			});
		}

		public override void Disconnect()
		{
			_buffer.QueueTask(() =>
			{
				if (_connection == null) throw ThrowDisconnected();

				_connection.Dispose();
				_connection = null;
				_eventHandler?.OnConnectionStateChange(false);
			});
		}

		async void ReadMessageThenQueue()
		{
			try
			{
				if (_connection == null) throw ThrowDisconnected();

				var result = await _connection.ReadMessage();
				if (result == null) break;

				_buffer.QueueTaskAsync(async () => await ProcessMessage(result));
			}
			catch (Exception ex) { HandleFail(ex); }

			// Repeatedly read messages, queuing up responses as a result.
			while (true)
			{
				
					
				
			}


			
		}

		async Task ProcessMessage(OBSDeserializedMessage? message)
		{
			if (_connection == null) throw ThrowDisconnected();

			// Perform the appropriate action
			var action = await _connection.ProcessMessage(message);
			switch (action)
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

			// Now that it's sorted, queue up the next message

		}

		public override void RefreshSpecs()
		{
			if (_connection == null) throw ThrowDisconnected();
			_eventHandler?.OnSpecsChange(_connection.CreateSpecs());
		}

		public override void RefreshConnectionStatus() => _eventHandler?.OnConnectionStateChange(_connection != null);

		public override void RefreshProgram(int mixBlock)
		{
			if (_connection == null) throw ThrowDisconnected();
			_eventHandler?.OnProgramValueChange(new(0, _connection.LookupCurrentProgramId(), new()));
		}

		public override void RefreshPreview(int mixBlock)
		{
			if (_connection == null) throw ThrowDisconnected();
			_eventHandler?.OnPreviewValueChange(new(0, _connection.LookupCurrentPreviewId(), new()));
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
