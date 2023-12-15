using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS
{
	public partial class OBSConnection : IDisposable
    {
		readonly OBSSwitcherConfig _config;
        readonly OBSWebsocketClient _client;
		readonly Dictionary<string, int> _nameToIdLookup = new();
		readonly Dictionary<int, string> _idToNameLookup = new();

		SceneData[]? _sceneData;
        bool? _isStudioMode;
		string? _currentPreview;
		string? _currentProgram;
		OBSProgramEventState _postProgramSetState = OBSProgramEventState.None;

        private OBSConnection(OBSSwitcherConfig config, OBSWebsocketClient client)
		{
			_config = config;
            _client = client;
        }

        public async Task<bool> ProcessDataUntilAllSpecsLoaded()
        {
			// Request needed info
			await _client.SendDatalessRequest(new OBSRequestMessage("GetSceneList", ""));
			await _client.SendDatalessRequest(new OBSRequestMessage("GetStudioModeEnabled", ""));
			await _client.SendDatalessRequest(new OBSRequestMessage("GetCurrentPreviewScene", ""));
			await _client.SendDatalessRequest(new OBSRequestMessage("GetCurrentProgramScene", ""));

			// Process incoming until we have all of it
			while (await ReadData() is not OBSSwitcherAction.Disconnected)
            {
				if (_sceneData != null && _isStudioMode != null && _currentPreview != null && _currentProgram != null) return true;
            }

			// We got disconnected
			return false;
        }

        public async Task<OBSSwitcherAction> ReadData()
        {
			// Get data
			var data = await _client.ReceiveData();
			if (data == null) return _client.IsConnected ? OBSSwitcherAction.None : OBSSwitcherAction.Disconnected;

			// Perform the appropriate action
			switch (data)
			{
				case OBSResponseMessage dMsg:
					// ValidateStatusCode(dMsg.Status); // We ignore failures to carry out requests because they're not critical.
					return dMsg.Data == null ? OBSSwitcherAction.None : ProcessResponseData(dMsg.Data);
				case OBSEventMessage eMsg:

					// If we recently set the program and we're waiting to change preview back to its old value (see the comment in SetProgram).
					switch (_postProgramSetState)
					{
						case OBSProgramEventState.WaitingForTransitionEnd:
							if (eMsg.EventType == "SceneTransitionEnded")
							{
								await _client.SendRequest(new OBSRequestMessage<CurrentPreviewSceneData>("SetCurrentPreviewScene", "", new(_currentPreview!)));
								_postProgramSetState = OBSProgramEventState.WaitingForPreviewCorrectionUpdate;
							}
							break;
						case OBSProgramEventState.WaitingForPreviewCorrectionUpdate:
							if (eMsg.EventType == "CurrentPreviewSceneChanged")
								_postProgramSetState = OBSProgramEventState.None;
							break;
					}

					return ProcessResponseData(eMsg.Data);
				default:
					throw new OBSCommunicationException("Unexpected data received from OBS");
			}
		}

		OBSSwitcherAction ProcessResponseData(OBSData? response)
		{
			// GetSceneList
			switch (response)
			{
				case SceneListData getSceneList:

					// Sort by index
					_sceneData = getSceneList.Scenes.OrderBy(s => s.SceneIndex).ToArray();

					// Create name translation tables for this.
					_nameToIdLookup.Clear();
					_idToNameLookup.Clear();
					for (int i = 0; i < _sceneData.Length; i++)
					{
						_nameToIdLookup.Add(_sceneData[i].SceneName, _sceneData[i].SceneIndex);
						_idToNameLookup.Add(_sceneData[i].SceneIndex, _sceneData[i].SceneName);
					}

					return OBSSwitcherAction.NotifySpecsChanged;

				case StudioModeEnabledData getStudioModeRaw:
					_isStudioMode = getStudioModeRaw.IsEnabled;
					return OBSSwitcherAction.NotifySpecsChanged;

				case CurrentPreviewSceneData previewRaw:
					if (_postProgramSetState != OBSProgramEventState.None) return OBSSwitcherAction.None;
					_currentPreview = previewRaw.SceneName;
					return OBSSwitcherAction.PreviewChanged;

				case CurrentProgramSceneData programRaw:
					_currentProgram = programRaw.SceneName;
					return OBSSwitcherAction.ProgramChanged;

				default:
					return OBSSwitcherAction.None;
			}
		}

		public int LookupCurrentPreviewId()
		{
			if (_currentPreview == null) throw UninitializedException();
			if (_nameToIdLookup.TryGetValue(_currentPreview, out int prevVal)) return prevVal;
			throw new OBSCommunicationException("Invalid preview ID received from OBS");
		}

		public int LookupCurrentProgramId()
		{
			if (_currentProgram == null) throw UninitializedException();
			if (_nameToIdLookup.TryGetValue(_currentProgram, out int progVal)) return progVal;
			throw new OBSCommunicationException("Invalid program ID received from OBS");
		}

        public async Task SetProgram(int mixBlock, int id)
        {
			if (_currentPreview == null) throw UninitializedException();

			// Annoyingly, when we're in studio mode, setting program can *also* update the preview like a cut operation.
			// This all depends on whether "Swap Preview/Output" is ticked in OBS, and there's no way to detect if it is.
			// To resolve this, we send a program signal, wait for a SceneTransitionEnded, *then* send a preview signal to reset
			// the preview back to its old value. While we wait for that final preview change to apply, we
			// suppress **any** preview events from being received (just suppress the first one post preview send).
			// TODO: Only do this stuff if studio mode is enabled? Playing it safe for now
			_postProgramSetState = OBSProgramEventState.WaitingForTransitionEnd;
			if (!_idToNameLookup.TryGetValue(id, out string? val)) throw new ArgumentException("Invalid ID given for setting program bus.");
			await _client.SendRequest(new OBSRequestMessage<CurrentProgramSceneData>("SetCurrentProgramScene", "", new(val)));
		}

		public async Task SetPreview(int mixBlock, int id)
		{
			if (!_idToNameLookup.TryGetValue(id, out string? val)) throw new ArgumentException("Invalid ID given for setting preview bus.");
			await _client.SendRequest(new OBSRequestMessage<CurrentProgramSceneData>("SetCurrentPreviewScene", "s", new(val)));
		}

		static void ValidateStatusCode(OBSResponseStatus status)
		{
			// status.Result is just shorthand to tell us whether status.Code is 100 or not (because OBS doesn't know how to design
			// efficient APIs I guess), so no need to check both.
			if (!status.Result) throw new OBSCommunicationException("OBS failed to retrieve requested data.");
		}

		public SwitcherSpecs CreateSpecs()
		{
			if (_sceneData == null || _isStudioMode == null) throw UninitializedException();

			// Create a list of inputs based on the scenes
			var features = new SwitcherMixBlockFeatures()
			{
				SupportsAutoAction = false,
				SupportsCutAction = false,
				SupportsCutBusAutoMode = false,
				SupportsCutBusCutMode = false,
				SupportsCutBusModeChanging = false,
				SupportsCutBusSwitching = false,
				SupportsDirectPreviewAccess = _isStudioMode.Value,
				SupportsDirectProgramModification = true
			};

			var inputs = new SwitcherBusInput[_sceneData.Length];
			for (int i = 0; i < inputs.Length; i++)
			{
				var scene = _sceneData[i];
				inputs[i] = new SwitcherBusInput(scene.SceneIndex, scene.SceneName);
			}

			return new SwitcherSpecs(true, new SwitcherMixBlock[]
			{
				_isStudioMode.Value ? SwitcherMixBlock.NewProgPrevSameInputs(features, inputs) : SwitcherMixBlock.NewCutBus(features, inputs)
			});
		}

		public static async Task<OBSConnection?> ConnectAndGetInfo(OBSSwitcherConfig config)
        {
            var client = new OBSWebsocketClient($"ws://{config.IP}:{config.Port}");
            await client.Connect();

            var connection = new OBSConnection(config, client);
			await connection.PerformHandshake();
			await connection.ProcessDataUntilAllSpecsLoaded();
            return connection;
        }

		Exception UninitializedException() => new OBSCommunicationException("Attempt to access OBS before specs fully gathered.");

		public void Dispose() => _client.Dispose();
	}

	public enum OBSProgramEventState
	{
		None,
		WaitingForTransitionEnd,
		WaitingForPreviewCorrectionUpdate,
	}

    public enum OBSSwitcherAction
    {
		None,
        Disconnected,
        NotifySpecsChanged,
		PreviewChanged,
		ProgramChanged,
    }
}
