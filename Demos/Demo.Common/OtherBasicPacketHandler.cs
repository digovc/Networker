using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Demo.Common
{
	public class OtherBasicPacketHandler : PacketHandlerBase<BasicPacket>
	{
		private readonly ILogger<OtherBasicPacketHandler> logger;

		public OtherBasicPacketHandler(ILogger<OtherBasicPacketHandler> logger)
		{
			this.logger = logger;
		}

		public override async Task Process(BasicPacket packet, IPacketContext context)
		{
			logger.LogDebug("Other Basic Packet Handler RUun");
		}
	}
}