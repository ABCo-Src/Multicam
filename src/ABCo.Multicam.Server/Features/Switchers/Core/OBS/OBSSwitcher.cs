using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
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
		readonly Dictionary<string, int> _sceneNameTable = new();

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

		public override async void Disconnect()
		{
			try
			{
				await _client.Disconnect();
				_isConnected = false;
				_eventHandler?.OnConnectionStateChange(false);
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		async void OBSEventLoop()
		{
			while (true)
			{
				try
				{
					// Get data, and stop if disconnected.
					var data = await _client.ReceiveData();
					if (data == null)
					{
						// Skip if it was just an unrecognised message
						if (_client.IsConnected)
							continue;

						// Stop if we've disconnected
						else
						{
							RefreshConnectionStatus();
							break;
						}
					}

					if (data is OBSResponseMessage msg)
					{
						ValidateStatus(msg.Status);
						ProcessResponse(msg.Data);
					}
					else if (data is OBSEventMessage msgEvent) ProcessResponse(msgEvent.Data);
					else throw new OBSCommunicationException("Unexpected data received from OBS");

				}
				catch (Exception ex) { HandleFail(ex); }
			}
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

		public override async void RefreshPreview(int mixBlock)
		{
			try
			{
				if (!_isConnected) ThrowDisconnected();
				await _client.SendDatalessRequest(new OBSRequestMessage("GetCurrentProgramScene", ""));
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override async void RefreshProgram(int mixBlock)
		{
			try
			{
				if (!_isConnected) ThrowDisconnected();
				await _client.SendDatalessRequest(new OBSRequestMessage("GetCurrentPreviewScene", ""));
			}
			catch (Exception ex) { HandleFail(ex); }
		}

		public override CutBusMode GetCutBusMode(int mixBlock) => CutBusMode.Cut;

		static void ValidateStatus(OBSResponseStatus status)
		{
			if (status.Code != 100 || !status.Result) throw new OBSCommunicationException("OBS failed to retrieve requested data.");
		}

		void ProcessResponse(OBSData response)
		{
			// GetSceneList
			switch (response)
			{
				case SceneListData getSceneList:

					// Sort by index
					var sortedSceneList = getSceneList.Scenes.OrderBy(s => s.SceneIndex).ToArray();

					// Create a list of inputs based on the scenes
					var inputs = new SwitcherBusInput[getSceneList.Scenes.Length];
					for (int i = 0; i < inputs.Length; i++)
					{
						var scene = sortedSceneList[i];
						inputs[i] = new SwitcherBusInput(scene.SceneIndex, scene.SceneName);
					}

					// Update the name-id translation table for this.
					_sceneNameTable.Clear();
					for (int i = 0; i < sortedSceneList.Length; i++)
						_sceneNameTable.Add(sortedSceneList[i].SceneName, sortedSceneList[i].SceneIndex);

					// Create new specs from this and report it
					_lastReceivedBusInputs = inputs;
					UpdateSpecsIfAllDataCollected();
					break;
				case StudioModeEnabledData getStudioModeRaw:
					_lastReceivedIsStudioMode = getStudioModeRaw.IsEnabled;
					UpdateSpecsIfAllDataCollected();
					break;
				case CurrentPreviewSceneData previewRaw:
					if (_sceneNameTable.TryGetValue(previewRaw.SceneName, out int prevVal))
						_eventHandler?.OnPreviewValueChange(new(0, prevVal, null));

					break;
				case CurrentProgramSceneData previewRaw:
					if (_sceneNameTable.TryGetValue(previewRaw.SceneName, out int progVal))
						_eventHandler?.OnProgramValueChange(new(0, progVal, null));

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

			_eventHandler?.OnSpecsChange(new SwitcherSpecs(true, new SwitcherMixBlock[]
			{
				isStudioMode ? SwitcherMixBlock.NewProgPrevSameInputs(features, busInputs) : SwitcherMixBlock.NewCutBus(features, busInputs)
			}));
		}

		public override void Dispose() => _client.Dispose();

		void HandleFail(Exception ex) => _eventHandler?.OnFailure(new SwitcherError(ex.Message));

		void ThrowDisconnected() => throw new OBSCommunicationException("Unexpected disconnection from OBS.");
	}
}
