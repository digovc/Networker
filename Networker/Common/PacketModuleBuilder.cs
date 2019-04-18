using System;
using System.Collections.Generic;
using System.Linq;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public class PacketModuleBuilder : IPacketModuleBuilder
	{
		public Dictionary<string, IPacketRegistry> PacketRegistries { get; set; }

		public PacketModuleBuilder()
		{
			PacketRegistries = new Dictionary<string, IPacketRegistry>();
		}

		public IPacketRegistry<T> ConfigurePacket<T>() where T : class
		{
			var typeName = typeof(T).Name;

			if (!PacketRegistries.ContainsKey(typeName))
			{
				var registry = new PacketRegistry<T>();
				PacketRegistries.Add(typeName, registry);
			}

			return PacketRegistries[typeName] as IPacketRegistry<T>;
		}

        public List<IPacketRegistry> GetPacketRegistries()
        {
            return PacketRegistries.Select(e => e.Value).ToList();
        }
    }
}