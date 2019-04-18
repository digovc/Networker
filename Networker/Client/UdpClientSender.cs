using System.Net;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Client
{
	public class UdpClientSender : IUdpSocketSender
	{
		private readonly IClient _client;

		public UdpClientSender(IClient client, IClientPacketProcessor clientPacketProcessor)
		{
			_client = client;
			clientPacketProcessor.SetUdpSocketSender(this);
		}

		public void Broadcast<T>(T packet) where T : PacketBase
        {
			_client.SendUdp(packet);
		}

		public void SendTo(byte[] packetBytes, IPEndPoint endpoint)
		{
			_client.SendUdp(packetBytes);
		}

		public void SendTo<T>(T packet, IPEndPoint endpoint) where T : PacketBase
        {
			_client.SendUdp(packet);
		}
	}
}