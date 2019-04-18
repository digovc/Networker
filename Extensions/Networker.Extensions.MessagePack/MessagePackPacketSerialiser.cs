﻿using MessagePack;
using Networker.Common.Abstractions;
using System;
using System.IO;
using System.Text;

namespace Networker.Extensions.MessagePack
{
	public class MessagePackPacketSerialiser : IPacketSerialiser
	{
		public bool CanReadLength => true;
		public bool CanReadName => true;

		public bool CanReadOffset => false;

		public T Deserialise<T>(byte[] packetBytes) where T : class
		{
			return MessagePackSerializer.Deserialize<T>(packetBytes);
		}

		public T Deserialise<T>(byte[] packetBytes, int offset, int length)
		{
			throw new NotImplementedException();
		}

		public byte[] Package(string name, byte[] bytes) 
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(memoryStream))
				{
					var nameBytes = Encoding.ASCII.GetBytes(name);
					binaryWriter.Write(nameBytes.Length);
					binaryWriter.Write(bytes.Length);
					binaryWriter.Write(nameBytes);
					binaryWriter.Write(bytes);
				}

				var packetBytes = memoryStream.ToArray();
				return packetBytes;
			}
		}

		public byte[] Serialise<T>(T packet) where T : PacketBase
        {
			using (var memoryStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(memoryStream))
				{
					var serialised = MessagePackSerializer.Serialize(packet);
					binaryWriter.Write(packet.Identifier);
					binaryWriter.Write(serialised.Length);
					binaryWriter.Write(serialised);
				}

				var packetBytes = memoryStream.ToArray();
				return packetBytes;
			}
		}
	}
}
