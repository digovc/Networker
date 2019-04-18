using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
	public interface IPacketModuleBuilder
	{
		IPacketRegistry<T> ConfigurePacket<T>() where T : class;
        List<IPacketRegistry> GetPacketRegistries();
    }
}
