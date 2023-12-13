using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
    public class OBSWebsocketClient : IDisposable
    {
        readonly Uri _hostname;
		readonly JsonSerializerOptions _deserializeOptions;
		ClientWebSocket? _client;

		byte[] _dataBuffer = new byte[128];

		public OBSWebsocketClient(string hostname)
		{
			_hostname = new Uri(hostname);
			_deserializeOptions = new JsonSerializerOptions();
			_deserializeOptions.Converters.Add(new OBSMessageJSONConverter());
			_deserializeOptions.Converters.Add(new OBSResponseJSONConverter());
		}

		public bool IsConnected => _client != null;

		public async Task Connect()
		{
			_client?.Dispose();
			_client = new();
			await _client.ConnectAsync(_hostname, CancellationToken.None);
		}

        public async Task<OBSDeserializedMessage?> ReceiveData()
		{
			int length = await ReadIntoBuffer();

			// Stop if nothing read or disconnected
			if (length == 0 || _client == null) return null;

			// Deserialize JSON payload
			return JsonSerializer.Deserialize<OBSDeserializedMessage>(_dataBuffer.AsSpan()[0..length], _deserializeOptions);
		}

		private async Task<int> ReadIntoBuffer()
		{
			// Don't do anything if we're no longer connected
			if (_client == null) return 0;

			int length = 0;
			while (true)
			{
				var res = await _client.ReceiveAsync(_dataBuffer.AsMemory()[length..], CancellationToken.None);
				length += res.Count;

				UpdateConnectionStatus();

				// If that's all the data, stop here. Otherwise, grow the buffer...
				if (res.EndOfMessage) break;
				Array.Resize(ref _dataBuffer, _dataBuffer.Length * 2);
			}
			return length;
		}

		void UpdateConnectionStatus()
		{
			if (_client?.State != WebSocketState.Open)
			{
				_client?.Dispose();
				_client = null;
			}
		}

		public async Task SendIdentify(OBSIdentifyMessage msg) => await SendAsync(new OBSSerializedMessage<OBSIdentifyMessage>(1, msg));
		public async Task SendIdentifyWithAuth(OBSIdentifyMessageWithAuth msg) => await SendAsync(new OBSSerializedMessage<OBSIdentifyMessageWithAuth>(1, msg));
		public async Task SendDatalessRequest(OBSRequestMessage msg) => await SendAsync(new OBSSerializedMessage<OBSRequestMessage>(6, msg));
		public async Task SendRequest<T>(OBSRequestMessage<T> msg) => await SendAsync(new OBSSerializedMessage<OBSRequestMessage<T>>(6, msg));
		public async Task Send(OBSRequestMessage msg) => await SendAsync(new OBSSerializedMessage<OBSRequestMessage>(7, msg));

		async Task SendAsync(object obj) => await _client.SendAsync(JsonSerializer.SerializeToUtf8Bytes(obj), WebSocketMessageType.Text, true, CancellationToken.None);

		public void Dispose() => _client.Dispose();
	}
}
