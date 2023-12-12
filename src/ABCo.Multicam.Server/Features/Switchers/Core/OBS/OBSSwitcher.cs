using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Requests;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.General;
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
		readonly OBSWebsocketClient _client;
		SwitcherBusInput[]? _lastReceivedBusInputs = null;
		bool? _lastReceivedIsStudioMode = null;

		bool _isConnected;

		public OBSSwitcher(OBSSwitcherConfig config)
		{
			_config = config;
			_client = new OBSWebsocketClient($"ws://{config.IP}:{config.Port}");
		}

		public override SwitcherPlatformCompatibilityValue GetPlatformCompatibility() => SwitcherPlatformCompatibilityValue.Supported;

		public override async void Connect()
		{
			// TODO: Slamming protection?
			try
			{
				// Connect to OBS
				await _client.Connect();
				await PerformHandshake();

				// Notify that we're connected
				_isConnected = true;
				_eventHandler?.OnConnectionStateChange(true);

				// Start listening for events async from OBS
				OBSEventLoop();
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		async void OBSEventLoop()
		{
			try
			{
				// Get data, and stop if disconnected.
				var data = await _client.ReceiveData();
				if (data == null)
				{
					_isConnected = false;
					return;
				}

				if (data is OBSResponseMessage msg)
				{
					ThrowMissingDataIf(msg.Status == null || msg.Status.Code == null || msg.Status.Result == null);
					if (msg.Status!.Code != 100) throw new OBSCommunicationException("OBS failed to fulfill data request.");
					ProcessResponse(msg);
				}
				else throw new OBSCommunicationException("Unexpected data received from OBS");
				
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override async void RefreshSpecs()
		{
			try
			{
				if (!_isConnected) ThrowDisconnected();
				await _client.SendDatalessRequest(new OBSRequestMessage("GetSceneList", ""));
				await _client.SendDatalessRequest(new OBSRequestMessage("GetStudioModeEnabled", ""));
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override void RefreshConnectionStatus()
		{
			_isConnected = _client.IsConnected;
			_eventHandler?.OnConnectionStateChange(_isConnected);
		}

		void ProcessResponse(OBSResponseMessage response)
		{
			// GetSceneList
			switch (response)
			{
				case OBSGetSceneListResponse getSceneListRaw:
					var data = getSceneListRaw.Data;
					ThrowMissingDataIf(getSceneListRaw.Data == null || getSceneListRaw.Data.Scenes == null);

					// Create a list of inputs based on the scenes
					var inputs = new SwitcherBusInput[data!.Scenes!.Length];
					for (int i = 0; i < inputs.Length; i++)
					{
						var scene = data.Scenes[i];
						ThrowMissingDataIf(scene.SceneIndex == null || scene.SceneName == "");
						inputs[i] = new SwitcherBusInput(data.Scenes[i].SceneIndex!.Value, data.Scenes[i].SceneName!);
					}

					// Create new specs from this and report it
					_lastReceivedBusInputs = inputs;
					UpdateSpecsIfAllDataCollected();
					break;
				case OBSGetStudioModeEnabledResponse getStudioModeRaw:
					ThrowMissingDataIf(getStudioModeRaw.Data == null || getStudioModeRaw.Data.IsEnabled == null);
					_lastReceivedIsStudioMode = getStudioModeRaw.Data!.IsEnabled;
					UpdateSpecsIfAllDataCollected();
					break;
			}
		}

		void UpdateSpecsIfAllDataCollected()
		{
			if (_lastReceivedBusInputs == null || _lastReceivedIsStudioMode == null) return;

			var busInputs = _lastReceivedBusInputs;
			var isStudioMode = _lastReceivedIsStudioMode.Value;

			var features = new SwitcherMixBlockFeatures()
			{
				SupportsAutoAction = isStudioMode,
				SupportsCutAction = false,
				SupportsCutBusAutoMode = false,
				SupportsCutBusCutMode = true,
				SupportsCutBusModeChanging = true,
				SupportsCutBusSwitching = true,
				SupportsDirectPreviewAccess = isStudioMode,
				SupportsDirectProgramModification = true
			};

			_eventHandler?.OnSpecsChange(new SwitcherSpecs(new SwitcherMixBlock[]
			{
				isStudioMode ? SwitcherMixBlock.NewProgPrevSameInputs(features, busInputs) : SwitcherMixBlock.NewCutBus(features, busInputs)
			}));
		}

		public override void Dispose() => _client.Dispose();

		void HandleFail(Exception ex) => _eventHandler?.OnFailure(new SwitcherError(ex.Message));

		void ThrowDisconnected() => throw new OBSCommunicationException("Unexpected disconnection from OBS.");
		void ThrowMissingDataIf(bool condition)
		{
			if (condition)
				throw new OBSCommunicationException("Missing JSON properties in data from OBS.");
		}
	}
}
