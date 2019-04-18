using System;
using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
	public interface IPacketRegistry
	{
		Type PacketType { get; }
		Type PacketHandlerType { get; }
        IPacketHandler PacketHandler { get; set; }
		int Identifier { get; }
	}

	public interface IPacketRegistry<T> : IPacketRegistry where T : class
	{
		IPacketRegistry<T> UseHandler<TPacketHandler>() where TPacketHandler : IPacketHandler;
	}
}