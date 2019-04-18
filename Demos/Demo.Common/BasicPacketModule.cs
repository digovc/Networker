using Networker.Common.Abstractions;

namespace Demo.Common
{
	public class BasicPacketModule : IPacketModule
	{
		public void Register(IPacketModuleBuilder builder)
		{
			builder.ConfigurePacket<BasicPacket>()
				.UseIdentifier(1);
		}
	}
}