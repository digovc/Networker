using System;
using System.Collections.Generic;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public class PacketRegistry<T> : IPacketRegistry<T> where T : class
	{
		public Type PacketType { get; private set; }
		public List<Type> PacketHandlerTypes { get; private set; }
		public object Identifier { get; private set; }
		public List<IPacketHandler> PacketHandlers { get; set; }

		public PacketRegistry()
		{
			PacketHandlerTypes = new List<Type>();
			PacketHandlers = new List<IPacketHandler>();
		}

		public IPacketRegistry<T> UseIdentifier(string identifier)
		{
			Identifier = identifier;
			return this;
		}

		public IPacketRegistry<T> UseHandler<TPacketHandler>() where TPacketHandler : IPacketHandler
		{
			PacketHandlerTypes.Add(typeof(T));
			return this;
		}
	}
}