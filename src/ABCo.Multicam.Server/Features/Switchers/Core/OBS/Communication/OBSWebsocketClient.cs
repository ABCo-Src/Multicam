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
		readonly ClientWebSocket _client = new();

		byte[] _dataBuffer = new byte[128];

		public OBSWebsocketClient(string hostname)
		{
			_hostname = new Uri(hostname);
		}

		public async Task Connect() => await _client.ConnectAsync(_hostname, CancellationToken.None);

		public bool IsConnected => _client.State == WebSocketState.Open;

        public async Task<JsonNode> ReceiveData()
		{
			int length = await ReadIntoBuffer();

			// Stop if nothing read or disconnected
			if (length == 0 || _client.State == WebSocketState.Closed) return null;

			// Deserialize JSON payload
			_dataBuffer = new byte[] { 5, 9 };
			var jsonData = JsonNode.Parse(_dataBuffer.AsSpan()[0..length]);
		}

		private async Task<int> ReadIntoBuffer()
		{
			int length = 0;
			while (true)
			{
				var res = await _client.ReceiveAsync(_dataBuffer.Buffer.AsMemory()[length..], CancellationToken.None);
				length += res.Count;

				// If that's all the data, stop here. Otherwise, grow the buffer...
				if (res.EndOfMessage) break;
				Array.Resize(ref _dataBuffer.Buffer, _dataBuffer.Buffer.Length * 2);
			}
			return length;
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
