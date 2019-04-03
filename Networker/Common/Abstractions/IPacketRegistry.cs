using System;
using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
	public interface IPacketRegistry
	{
		Type PacketType { get; }
		List<Type> PacketHandlerTypes { get; }
		object Identifier { get; }
	}

	public interface IPacketRegistry<T> : IPacketRegistry where T : class
	{
		IPacketRegistry<T> UseIdentifier(string identifier);
		IPacketRegistry<T> UseHandler<TPacketHandler>() where TPacketHandler : IPacketHandler;
	}
}