using System;
using Networker.Common.Abstractions;

namespace Demo.Common
{
	public class BasicPacket : PacketBase
	{
        public string StringData { get; set; }

        public override int Identifier => (int)PacketIdentifiers.BasicPacket;
    }
}
