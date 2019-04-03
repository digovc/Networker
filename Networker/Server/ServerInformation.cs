using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class ServerInformation : IServerInformation
    {
        public bool IsRunning { get; set; }

        public long InvalidTcpPackets { get; set; }
        public long ProcessedTcpPackets { get; set; }

        public long ProcessedUdpPackets { get; set; }
        public long TcpBytes { get; set; }
        public long UdpBytes { get; set; }
        public long InvalidUdpPackets { get; set; }
    }
}
