using System;

namespace Networker.Server
{
	public class ServerInformationEventArgs : EventArgs
	{
		public long InvalidTcpPackets { get; set; }
		public long InvalidUdpPackets { get; set; }
		public long ProcessedTcpPackets { get; set; }
		public long ProcessedUdpPackets { get; set; }
		public long TcpConnections { get; set; }
		public long TcpBytes { get; set; }
		public long UdpBytes { get; set; }
	}
}