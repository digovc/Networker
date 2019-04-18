using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class ServerPacketProcessor : IServerPacketProcessor
	{
		private readonly ILogger logger;
		private readonly List<IMiddlewareHandler> middlewares;
		private readonly ObjectPool<IPacketContext> packetContextObjectPool;
		private readonly IPacketSerialiser packetSerialiser;
		private readonly IServerInformation serverInformation;
		private readonly ObjectPool<ISender> tcpSenderObjectPool;
		private readonly ObjectPool<ISender> udpSenderObjectPool;
		private IUdpSocketSender _socketSender;
        private readonly IPacketRegistry[] _packetRegistries;

		public ServerPacketProcessor(ServerBuilderOptions options,
			ILogger<ServerPacketProcessor> logger,
			IPacketHandlers packetHandlers,
			IServerInformation serverInformation,
			IPacketSerialiser packetSerialiser,
			IEnumerable<IMiddlewareHandler> middlewares,
            IPacketModuleBuilder packetModuleBuilder)
		{
			this.logger = logger;
			this.serverInformation = serverInformation;
			this.packetSerialiser = packetSerialiser;
			this.middlewares = middlewares.ToList();

			tcpSenderObjectPool = new ObjectPool<ISender>(options.TcpMaxConnections);

			for (var i = 0; i < tcpSenderObjectPool.Capacity; i++)
				tcpSenderObjectPool.Push(new TcpSender(packetSerialiser));

			udpSenderObjectPool = new ObjectPool<ISender>(options.TcpMaxConnections * 2);

			for (var i = 0; i < udpSenderObjectPool.Capacity; i++)
				udpSenderObjectPool.Push(new UdpSender(_socketSender));

			packetContextObjectPool = new ObjectPool<IPacketContext>(options.TcpMaxConnections * 2);

			for (var i = 0; i < packetContextObjectPool.Capacity; i++)
				packetContextObjectPool.Push(new PacketContext
				{
					Serialiser = packetSerialiser
				});


            var registries = packetModuleBuilder.GetPacketRegistries();

            _packetRegistries = new IPacketRegistry[registries.Max(e => e.Identifier)];

            foreach (var packetRegistry in registries)
            {
                if (_packetRegistries[packetRegistry.Identifier] != null)
                {
                    //already exists
                }

                _packetRegistries[packetRegistry.Identifier] = packetRegistry;
            }
		}

		public async Task ProcessFromBuffer(ISender sender,
			byte[] buffer,
			int offset = 0,
			int length = 0,
			bool isTcp = true)
		{
			var bytesRead = 0;
			var currentPosition = offset;

			if (length == 0)
				length = buffer.Length;

			while (bytesRead < length)
            {
                var packetIdentifier = BitConverter.ToInt32(buffer, currentPosition);
                currentPosition += 4;

                var packetSize = packetSerialiser.CanReadLength
					? BitConverter.ToInt32(buffer, currentPosition)
					: 0;

				if (packetSerialiser.CanReadLength) currentPosition += 4;

				try
                {
                    var packetRegistry = _packetRegistries[packetIdentifier];
                    var packetContext = packetContextObjectPool.Pop();
					packetContext.Sender = sender;
					packetContext.Registry = packetRegistry;

                    Array.Clear(packetContext.PacketBytes, 0, packetContext.PacketBytes.Length);
                    Buffer.BlockCopy(buffer, currentPosition, packetContext.PacketBytes, 0, packetSize);

                    foreach (var registryPacketHandlerType in packetContext.Registry.PacketHandlers)
                    {
                        await registryPacketHandlerType.Handle(packetContext);
                    }

					packetContextObjectPool.Push(packetContext);
				}
				catch (Exception e)
				{
					logger.Error(e);
				}

				if (packetSerialiser.CanReadLength) currentPosition += packetSize;

				bytesRead += packetSize;
                
				if (packetSerialiser.CanReadLength) bytesRead += 4;

				if (isTcp)
					serverInformation.ProcessedTcpPackets++;
				else
					serverInformation.ProcessedUdpPackets++;
			}

			if (isTcp)
				serverInformation.TcpBytes += buffer.Length;
			else
			{
				serverInformation.UdpBytes += buffer.Length;
			}
		}

		public void ProcessTcp(SocketAsyncEventArgs socketEvent)
		{
			var sender = tcpSenderObjectPool.Pop() as TcpSender;
			try
			{
				sender.Socket = ((AsyncUserToken) socketEvent.UserToken).Socket;
				ProcessPacketsFromSocketEventArgs(sender, socketEvent);
			}
			catch (Exception e)
			{
				logger.Error(e);
			}

			tcpSenderObjectPool.Push(sender);
		}

		public void ProcessUdp(SocketAsyncEventArgs socketEvent)
		{
			var sender = udpSenderObjectPool.Pop() as UdpSender;
			try
			{
				sender.RemoteEndpoint = socketEvent.RemoteEndPoint as IPEndPoint;
				ProcessPacketsFromSocketEventArgs(sender, socketEvent, false);
			}
			catch (Exception e)
			{
				logger.Error(e);
			}

			udpSenderObjectPool.Push(sender);
		}

		public void ProcessUdpFromBuffer(EndPoint endPoint, byte[] buffer, int offset = 0, int length = 0)
		{
			var sender = udpSenderObjectPool.Pop() as UdpSender;
			try
			{
				sender.RemoteEndpoint = endPoint as IPEndPoint;

				ProcessFromBuffer(sender, buffer, offset, length, false)
					.GetAwaiter()
					.GetResult();
			}
			catch (Exception e)
			{
				logger.Error(e);
			}

			udpSenderObjectPool.Push(sender);
		}

		public void SetUdpSender(IUdpSocketSender sender)
		{
			_socketSender = sender;
		}

		private void ProcessPacketsFromSocketEventArgs(ISender sender,
			SocketAsyncEventArgs eventArgs,
			bool isTcp = true)
		{
			ProcessFromBuffer(sender,
					eventArgs.Buffer,
					eventArgs.Offset,
					eventArgs.BytesTransferred,
					isTcp)
				.GetAwaiter()
				.GetResult();
		}
	}
}