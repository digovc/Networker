using System;
using System.Linq;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Client
{
    public class ClientPacketProcessor : IClientPacketProcessor
    {
        private readonly ObjectPool<byte[]> bytePool;
        private readonly ILogger logger;
        private readonly ClientBuilderOptions options;
        private readonly IPacketSerialiser packetSerialiser;
        private readonly ObjectPool<ISender> tcpSenderObjectPool;
        private readonly ObjectPool<ISender> udpSenderObjectPool;
        private readonly ObjectPool<IPacketContext> _packetContextObjectPool;
        private readonly IPacketRegistry[] _packetRegistries;

        private IUdpSocketSender _udpSocketSender;

        public ClientPacketProcessor(ClientBuilderOptions options,
            IPacketSerialiser packetSerialiser,
            ILogger<ClientPacketProcessor> logger,
            IPacketModuleBuilder packetModuleBuilder)
        {
            this.options = options;
            this.packetSerialiser = packetSerialiser;
            this.logger = logger;

            tcpSenderObjectPool = new ObjectPool<ISender>(options.ObjectPoolSize);

            for (var i = 0; i < tcpSenderObjectPool.Capacity; i++)
                tcpSenderObjectPool.Push(new TcpSender(packetSerialiser));

            udpSenderObjectPool = new ObjectPool<ISender>(options.ObjectPoolSize);

            for (var i = 0; i < udpSenderObjectPool.Capacity; i++)
                udpSenderObjectPool.Push(new UdpSender(_udpSocketSender));

            _packetContextObjectPool = new ObjectPool<IPacketContext>(options.ObjectPoolSize * 2);

            for (var i = 0; i < _packetContextObjectPool.Capacity; i++)
                _packetContextObjectPool.Push(new PacketContext
                {
                    Serialiser = this.packetSerialiser
                });

            bytePool = new ObjectPool<byte[]>(options.ObjectPoolSize);

            for (var i = 0; i < bytePool.Capacity; i++) bytePool.Push(new byte[options.PacketSizeBuffer]);

            var registries = packetModuleBuilder.GetPacketRegistries();

            if (registries.Any())
            {
                _packetRegistries = new IPacketRegistry[registries.Max(e => e.Identifier) + 1];

                foreach (var packetRegistry in registries)
                {
                    if (_packetRegistries[packetRegistry.Identifier] != null)
                    {
                        //already exists
                    }

                    _packetRegistries[packetRegistry.Identifier] = packetRegistry;
                }
            }
        }

        public void Process(Socket socket)
        {
            var buffer = bytePool.Pop();
            var sender = tcpSenderObjectPool.Pop();

            try
            {
                socket.Receive(buffer);

                var tcpSender = sender as TcpSender;
                tcpSender.Socket = socket;

                Process(buffer, sender);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                bytePool.Push(buffer);
                tcpSenderObjectPool.Push(sender);
            }
        }

        public void Process(UdpReceiveResult data)
        {
            var sender = udpSenderObjectPool.Pop();

            try
            {
                var buffer = data.Buffer;

                var udpSender = sender as UdpSender;
                udpSender.RemoteEndpoint = data.RemoteEndPoint;

                Process(buffer, sender);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                udpSenderObjectPool.Push(sender);
            }
        }

        public void SetUdpSocketSender(IUdpSocketSender socketSender)
        {
            _udpSocketSender = socketSender;
        }

        private void Process(byte[] buffer, ISender sender)
        {
            var bytesRead = 0;
            var currentPosition = 0;

            while (bytesRead < buffer.Length)
            {
                var packetRegistry = _packetRegistries[BitConverter.ToInt32(buffer, currentPosition)];

                currentPosition += 4;

                var packetSize = packetSerialiser.CanReadLength
                    ? BitConverter.ToInt32(buffer, currentPosition)
                    : 0;

                if (packetSerialiser.CanReadLength) currentPosition += 4;

                if (packetSerialiser.CanReadLength && buffer.Length - bytesRead < packetSize)
                {
                    logger.Error(new Exception("Packet was lost"));
                    return;
                }

                var context = _packetContextObjectPool.Pop();
                context.Sender = sender;

                Array.Clear(context.PacketBytes, 0, context.PacketBytes.Length);
                Buffer.BlockCopy(buffer, currentPosition, context.PacketBytes, 0, packetSize);
                
                packetRegistry.PacketHandler.Handle(context);

                _packetContextObjectPool.Push(context);

                currentPosition += packetSize;
                currentPosition += 4;//ID
                bytesRead += packetSize;

                if (packetSerialiser.CanReadLength) bytesRead += 4;
            }
        }
    }
}