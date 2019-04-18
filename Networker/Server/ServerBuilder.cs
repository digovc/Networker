using Microsoft.Extensions.DependencyInjection;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;
using System;
using System.Collections.Generic;
using Networker.Common;

namespace Networker.Server
{
    public class ServerBuilder : BuilderBase<IServerBuilder, IServer, ServerBuilderOptions>, IServerBuilder
    {
        private Type tcpSocketListenerFactory;
        private Type udpSocketListenerFactory;

        public ServerBuilder() : base()
        {

        }

        public override IServer Build()
        {
            this.SetupSharedDependencies();

            this.serviceCollection.AddSingleton<IPacketModuleBuilder, PacketModuleBuilder>();
            this.serviceCollection.AddSingleton<ITcpConnections, TcpConnections>();
            this.serviceCollection.AddSingleton<IServer, Server>();
            this.serviceCollection.AddSingleton<IServerInformation, ServerInformation>();
            this.serviceCollection.AddSingleton<IServerPacketProcessor, ServerPacketProcessor>();
            this.serviceCollection.AddSingleton<IBufferManager>(new BufferManager(
                this.options.PacketSizeBuffer * this.options.TcpMaxConnections * 5,
                this.options.PacketSizeBuffer));
            this.serviceCollection.AddSingleton<IUdpSocketSender, UdpSocketSender>();

            if (this.tcpSocketListenerFactory == null)
                this.serviceCollection
                    .AddSingleton<ITcpSocketListenerFactory, DefaultTcpSocketListenerFactory>();

            if (this.udpSocketListenerFactory == null)
            {
                this.serviceCollection
                    .AddSingleton<IUdpSocketListenerFactory, DefaultUdpSocketListenerFactory>();
            }

            this.serviceCollection.AddSingleton<IUdpSocketListener>(collection => collection.GetService<IUdpSocketListenerFactory>().Create());

            var serviceProvider = this.GetServiceProvider();

            foreach (var module in serviceProvider.GetService<IEnumerable<IPacketModule>>())
            {
                module.Register(serviceProvider.GetService<IPacketModuleBuilder>());
            }

            return serviceProvider.GetService<IServer>();
        }

        public IServerBuilder SetMaximumConnections(int maxConnections)
        {
            this.options.TcpMaxConnections = maxConnections;
            return this;
        }

        public IServerBuilder UseTcpSocketListener<T>()
            where T : class, ITcpSocketListenerFactory
        {
            this.tcpSocketListenerFactory = typeof(T);
            this.serviceCollection.AddSingleton<ITcpSocketListenerFactory, T>();
            return this;
        }

        public IServerBuilder UseUdpSocketListener<T>()
            where T : class, IUdpSocketListenerFactory
        {
            this.udpSocketListenerFactory = typeof(T);
            this.serviceCollection.AddSingleton<IUdpSocketListenerFactory, T>();
            return this;
        }
    }
}