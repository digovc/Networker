using System;
using Networker.Common.Abstractions;
using Networker.Extensions.Json;

namespace Networker.Tests.ServerPerformance
{
	public class DoNothingSerialiser : IPacketSerialiser
	{
		private readonly byte[] _bytes;
		private readonly PerformancePacket _packet;

		public DoNothingSerialiser()
		{
			_packet = new PerformancePacket();
			_bytes = new JsonSerialiser().Serialise(_packet);
		}

		public bool CanReadLength => true;

		public bool CanReadName => true;

		public bool CanReadOffset => false;

		public T Deserialise<T>(byte[] packetBytes) where T : class
		{
			return _packet as T;
		}

		public T Deserialise<T>(byte[] packetBytes, int offset, int length)
		{
			throw new NotImplementedException();
		}

		public byte[] Package(string name, byte[] bytes)
		{
			throw new NotImplementedException();
		}

		public byte[] Serialise<T>(T packet) where T : PacketBase
        {
			return _bytes;
		}
	}
}