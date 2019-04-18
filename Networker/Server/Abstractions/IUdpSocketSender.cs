using System.Net;
using Networker.Common.Abstractions;

namespace Networker.Server.Abstractions
{
    public interface IUdpSocketSender
    {
        void SendTo(byte[] packetBytes, IPEndPoint endpoint);
        void SendTo<T>(T packet, IPEndPoint endpoint) where T : PacketBase;
        void Broadcast<T>(T packet) where T : PacketBase;
    }
}
