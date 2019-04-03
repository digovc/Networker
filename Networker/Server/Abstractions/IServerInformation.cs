namespace Networker.Server.Abstractions
{
    public interface IServerInformation
    {
        bool IsRunning { get; set; }

        long InvalidTcpPackets { get; set; }
        long ProcessedTcpPackets { get; set; }

        long InvalidUdpPackets { get; set; }
        long ProcessedUdpPackets { get; set; }
        long TcpBytes { get; set; }
        long UdpBytes { get; set; }
    }
}
