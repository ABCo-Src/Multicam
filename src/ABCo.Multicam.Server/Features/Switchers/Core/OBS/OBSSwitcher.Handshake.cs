using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS
{
	public partial class OBSSwitcher
	{
		async Task PerformHandshake()
		{
			var helloMsg = await GetAndValidateHelloMessage();
			await SendIdentifyMessage(helloMsg);
			await GetAndValidateIdentifiedMessage();
		}

		async Task<OBSHelloMessage> GetAndValidateHelloMessage()
		{
			var rawMsg = await _client.ReceiveData();

			// Verify type is correct
			if ((int?)rawMsg["op"] != 0) throw new OBSCommunicationException("Unexpected message from OBS when performing handshake.");

			// Verify all data is present
			ThrowMissingDataIf(rawMsg["op"] == null || rawMsg["obsWebSocketVersion"] == null || rawMsg["rpcVersion"] == null);
			if (rawMsg["Auth"] != null)
			{
				var auth = rawMsg["Auth"]!;
				ThrowMissingDataIf(auth["Challenge"] == null || auth["Salt"] == null));
			}

			return msg;
		}

		async Task SendIdentifyMessage(OBSHelloMessage helloMsg)
		{
			if (helloMsg.Auth == null)
			{
				var msg = new OBSIdentifyMessage();
				Populate(msg);
				await _client.SendIdentify(msg);
			}
			else
			{
				var msg = new OBSIdentifyMessageWithAuth() { Authentication = GenerateAuthenticationCode(_config.Password, helloMsg.Auth!.Challenge!, helloMsg.Auth!.Salt!) };
				Populate(msg);
				await _client.SendIdentifyWithAuth(msg);
			}

			static void Populate(OBSIdentifyMessage msg)
			{
				msg.EventSubscriptions = 33; // EventSubscription::All (for now)
				msg.RPCVersion = 1;
			}
		}

		async Task GetAndValidateIdentifiedMessage()
		{
			var rawMsg = await _client.ReceiveData();
			if (rawMsg is not OBSIdentifiedMessage msg) throw new OBSCommunicationException("Failed to finish OBS handshake, verify the Server Password given is correct.");

		}

		static string GenerateAuthenticationCode(string password, string challenge, string salt)
		{
			string concat = password + salt;
			byte[] sha256hash = SHA256.HashData(Encoding.UTF8.GetBytes(concat));
			string sha256HashWithChallenge = Convert.ToBase64String(sha256hash) + challenge;
			byte[] sha256HashOfSha256HashWithChallenge = SHA256.HashData(Encoding.UTF8.GetBytes(sha256HashWithChallenge));
			return Convert.ToBase64String(sha256HashOfSha256HashWithChallenge);
		}
	}
}
