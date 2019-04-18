using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public class PacketRegistry<T> : IPacketRegistry<T> where T : PacketBase
	{
		public Type PacketType { get; private set; }
		public Type PacketHandlerType { get; private set; }
		public int Identifier { get; private set; }
		public IPacketHandler PacketHandler { get; set; }

		public PacketRegistry()
		{
            Identifier = ((T) Activator.CreateInstance<T>()).Identifier;
        }
        
		public IPacketRegistry<T> UseHandler<TPacketHandler>() where TPacketHandler : IPacketHandler
		{
			PacketHandlerType = typeof(TPacketHandler);
			return this;
		}
	}
}