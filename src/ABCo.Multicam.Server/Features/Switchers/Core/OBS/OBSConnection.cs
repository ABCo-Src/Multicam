﻿using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
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
        string? _currentTransition;

        TaskCompletionSource? _waitForTransitionEnd;
        TaskCompletionSource? _waitForRequestResponse;

        private OBSConnection(OBSSwitcherConfig config, OBSWebsocketClient client)
        {
            _config = config;
            _client = client;
        }

        public async Task ProcessDataUntilAllSpecsLoaded()
        {
            // Request needed info
            await _client.SendDatalessRequest(new OBSRequestMessage("GetSceneList", ""));
            await _client.SendDatalessRequest(new OBSRequestMessage("GetStudioModeEnabled", ""));
            await _client.SendDatalessRequest(new OBSRequestMessage("GetCurrentSceneTransition", ""));
            await _client.SendDatalessRequest(new OBSRequestMessage("GetCurrentProgramScene", ""));

            // Wait until this information comes (the preview will also be requested by the message processor if studio mode is enabled)
            while (true)
            {
                await ReadAndProcessMessage();
                if (_sceneData != null && _isStudioMode != null && _currentTransition != null && _currentProgram != null) return;
            }
        }

        async Task ReadUntilPreview()
        {
            // Request preview
            await _client.SendDatalessRequest(new OBSRequestMessage("GetCurrentPreviewScene", ""));

            // Wait for preview to come through
            while (true)
            {
                await ReadAndProcessMessage();
                if (_currentPreview != null) return;
            }
        }

        public async Task<OBSSwitcherAction> ReadAndProcessMessage()
        {
            // Get data
            var data = await _client.ReadMessage();
            if (data == null) return _client.IsConnected ? OBSSwitcherAction.None : throw OBSCommunicationException.UnexpectedDisconnection;

            // Perform the appropriate action
            switch (data)
            {
                case OBSResponseMessage dMsg:

                    if (_waitForRequestResponse != null)
                    {
                        _waitForRequestResponse.SetResult();
                        _waitForRequestResponse = null;
                    }

                    ValidateStatusCode(dMsg.Status);
                    return dMsg.Data == null ? OBSSwitcherAction.None : await ProcessResponseData(dMsg.Data);
                case OBSEventMessage eMsg:

                    // If we're waiting for the scene transition to end, report that it has.
                    if (_waitForTransitionEnd != null && eMsg.EventType == "SceneTransitionEnded")
                    {
                        _waitForTransitionEnd.SetResult();
                        _waitForTransitionEnd = null;
                    }

                    return await ProcessResponseData(eMsg.Data);
                default:
                    throw new OBSCommunicationException("Unexpected data received from OBS");
            }
        }

        async Task<OBSSwitcherAction> ProcessResponseData(OBSData? response)
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

                    // If we just switched *into* studio mode, we'll ask for and wait for the preview to come through first, then once that happens 
                    // we can notify of the specs change and that'll refresh everything (so if other state changed while waiting that's fine).
                    if (getStudioModeRaw.IsEnabled)
                    {
                        _currentPreview = null;
                        await ReadUntilPreview();
                        _isStudioMode = true;
                    }
                    else
                        _isStudioMode = false;
                    
                    return OBSSwitcherAction.NotifySpecsChanged;

                case CurrentPreviewSceneData previewRaw:
                    _currentPreview = previewRaw.SceneName;
                    return OBSSwitcherAction.PreviewChanged;

                case CurrentProgramSceneData programRaw:
                    _currentProgram = programRaw.SceneName;
                    return OBSSwitcherAction.ProgramChanged;

                case CurrentSceneTransition sceneTransition:
                    _currentTransition = sceneTransition.TransitionName;
                    return OBSSwitcherAction.None;

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
            // So we don't use this function outside of it - instead opting to let the emulator above the switcher set preview and perform "cut".
            if (!_idToNameLookup.TryGetValue(id, out string? val)) throw new ArgumentException("Invalid ID given for setting program bus.");
            await SendRequestAndWaitForResponse(new OBSRequestMessage<CurrentProgramSceneData>("SetCurrentProgramScene", "", new(val)));
        }

        public async Task SetPreview(int mixBlock, int id)
        {
            Debug.WriteLine("Pre" + _idToNameLookup[id]);

            if (!_idToNameLookup.TryGetValue(id, out string? val)) throw new ArgumentException("Invalid ID given for setting preview bus.");
            await SendRequestAndWaitForResponse(new OBSRequestMessage<CurrentProgramSceneData>("SetCurrentPreviewScene", "s", new(val)));
        }

        // Only usable in studio mode
        public async Task Cut(int mixBlock)
        {
            if (_currentTransition == null) throw UninitializedException();
            string oldTransition = _currentTransition;

            // Switch to cut if needed
            if (oldTransition != "Cut")
                await SendRequestAndWaitForResponse(new OBSRequestMessage<CurrentSceneTransition>("SetCurrentSceneTransition", "", new("Cut")));

            // Transition
            _waitForTransitionEnd = new();
            await _client.SendDatalessRequest(new OBSRequestMessage("TriggerStudioModeTransition", ""));
            await _waitForTransitionEnd.Task;

            // Switch back to previous
            if (oldTransition != "Cut")
                await SendRequestAndWaitForResponse(new OBSRequestMessage<CurrentSceneTransition>("SetCurrentSceneTransition", "", new(oldTransition)));
        }

        async Task SendRequestAndWaitForResponse<T>(OBSRequestMessage<T> data)
        {
            _waitForRequestResponse = new();
            await _client.SendRequest(data);
            await _waitForRequestResponse.Task;
            _waitForRequestResponse = null;
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

            var features = new SwitcherMixBlockFeatures()
            {
                SupportsAutoAction = false,
                SupportsCutAction = _isStudioMode.Value,
                SupportsCutBusAutoMode = false,
                SupportsCutBusCutMode = false,
                SupportsCutBusModeChanging = false,
                SupportsCutBusSwitching = false,
                SupportsDirectPreviewAccess = _isStudioMode.Value,
                SupportsDirectProgramModification = false
            };

            // Create a list of inputs based on the scenes
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
        ProgramChanged
    }
}
