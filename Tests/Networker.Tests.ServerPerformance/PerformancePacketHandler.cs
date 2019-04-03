using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Tests.ServerPerformance
{
	public class PerformancePacketHandler : PacketHandlerBase<PerformancePacket>
	{
		private readonly ILogger<PerformancePacketHandler> logger;
		private readonly IServerInformation serverInformation;
		private int count = 0;

		public PerformancePacketHandler(ILogger<PerformancePacketHandler> logger, IServerInformation serverInformation)
		{
			this.logger = logger;
			this.serverInformation = serverInformation;
		}

		public override async Task Process(PerformancePacket packet, IPacketContext context)
		{
			//count++;
			//if(count % 10000 == 0)
			//logger.LogDebug($"Run: {count}. Total bytes: {serverInformation.TcpBytes}");
		}
	}
}