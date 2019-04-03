using Demo.Common;
using Networker.Common.Abstractions;

namespace Demo.PacketModule
{
	public class BasicServerPacketModule : IPacketModule
	{
		public void Register(IPacketModuleBuilder builder)
		{
			builder.ConfigurePacket<BasicPacket>()
				.UseHandler<BasicPacketHandler>()
				.UseHandler<OtherBasicPacketHandler>();
		}
	}
}