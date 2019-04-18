using MessagePack;
using System;
using Networker.Common.Abstractions;

namespace Networker.Tests.MessagePack 
{
	[MessagePackObject]
    public class PingPacket : PacketBase
    {
		[Key(0)]
		public virtual DateTime Time { get; set; }

		public PingPacket(DateTime time) 
		{
			Time = time;
		}
    }
}
