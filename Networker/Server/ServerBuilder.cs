using System;
using Microsoft.Extensions.DependencyInjection;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class ServerBuilder : BuilderBase<IServerBuilder, IServer, ServerBuilderOptions>, IServerBuilder
    {
        private Type tcpSocketListenerFactory;
        private Type udpSocketListenerFactory;

        public override IServer Build()
        {
            SetupSharedDependencies();
            serviceCollection.AddSingleton<ITcpConnections, TcpConnections>();
            serviceCollection.AddSingleton<IServer, Server>();
            serviceCollection.AddSingleton<IServerInformation, ServerInformation>();
            serviceCollection.AddSingleton<IServerPacketProcessor, ServerPacketProcessor>();
            serviceCollection.AddSingleton<IBufferManager>(new BufferManager(
                options.PacketSizeBuffer * options.TcpMaxConnections * 5,
                options.PacketSizeBuffer));
            serviceCollection.AddSingleton<IUdpSocketSender, UdpSocketSender>();

            if (tcpSocketListenerFactory == null)
                serviceCollection
                    .AddSingleton<ITcpSocketListenerFactory, DefaultTcpSocketListenerFactory>();

            if (udpSocketListenerFactory == null)
                serviceCollection
                    .AddSingleton<IUdpSocketListenerFactory, DefaultUdpSocketListenerFactory>();

            serviceCollection.AddSingleton(collection => collection.GetService<IUdpSocketListenerFactory>().Create());

            var serviceProvider = GetServiceProvider();

            foreach (var packetRegistry in serviceProvider.GetService<IPacketModuleBuilder>().GetPacketRegistries())
            {
                var packetHandler = serviceProvider.GetService(packetRegistry.PacketHandlerType);
                packetRegistry.PacketHandler = packetHandler as IPacketHandler;
            }

            return serviceProvider.GetService<IServer>();
        }

        public IServerBuilder SetMaximumConnections(int maxConnections)
        {
            options.TcpMaxConnections = maxConnections;
            return this;
        }

        public IServerBuilder UseTcpSocketListener<T>()
            where T : class, ITcpSocketListenerFactory
        {
            tcpSocketListenerFactory = typeof(T);
            serviceCollection.AddSingleton<ITcpSocketListenerFactory, T>();
            return this;
        }

        public IServerBuilder UseUdpSocketListener<T>()
            where T : class, IUdpSocketListenerFactory
        {
            udpSocketListenerFactory = typeof(T);
            serviceCollection.AddSingleton<IUdpSocketListenerFactory, T>();
            return this;
        }
    }
}